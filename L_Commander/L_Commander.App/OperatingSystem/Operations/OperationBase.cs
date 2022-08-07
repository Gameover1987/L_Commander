using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations;

public abstract class OperationBase<TUnit> : IFileSystemOperation
    where TUnit : class, IUnitOfWork
{
    protected readonly ConcurrentQueue<TUnit> _worksQueue = new ConcurrentQueue<TUnit>();
    private readonly ConcurrentBag<Exception> _errors = new ConcurrentBag<Exception>();
    protected readonly ConcurrentBag<TUnit> _activeWorks = new ConcurrentBag<TUnit>();

    private CancellationTokenSource _cancellationTokenSource;
    protected bool _isInitialized;

    public bool IsStarted { get; protected set; }

    public bool HasErrors
    {
        get { return _errors.Any(); }
    }

    public Exception[] Errors
    {
        get { return _errors.ToArray(); }
    }

    public event EventHandler<OperationProgressEventArgs> TotalProgress;

    public Task Execute()
    {
        _activeWorks.Clear();
        _worksQueue.Clear();
        _errors.Clear();

        if (!_isInitialized)
            throw new ArgumentException($"{GetType().Name} instance is not initialized!");

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return Task.Run(() =>
        {
            IsStarted = true;
            try
            {
                Setup();

                var tasks = new List<Task>();
                for (var i = 0; i < Environment.ProcessorCount / 2; i++)
                {
                    tasks.Add(Task.Run(() => ProcessQueue(cancellationToken), cancellationToken));
                }

                Task.WaitAll(tasks.ToArray());

                Cleanup();
            }
            catch (Exception exception)
            {
                _errors.Add(exception);
            }
            finally
            {
                IsStarted = false;
            }

            return Task.CompletedTask;
        }, cancellationToken);
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();

        _worksQueue.Clear();
    }

    protected abstract void Setup();

    protected virtual void Cleanup()
    {

    }

    protected abstract OperationProgressEventArgs GetProgressEventArgs(TUnit unitOfWork);

    protected void NotifyProgress(TUnit unitOfWork)
    {
        TotalProgress?.Invoke(this, GetProgressEventArgs(unitOfWork));
    }

    protected virtual void NotifyActiveItemProgress()
    {

    }

    private void ProcessQueue(CancellationToken cancellationToken)
    {
        while (!_worksQueue.IsEmpty)
        {
            _worksQueue.TryDequeue(out var work);

            if (work == null)
                return;

            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                work.Progress += WorkOnProgress;
                work.Do(cancellationToken);

                NotifyProgress(work);
            }
            catch (Exception exception)
            {
                _errors.Add(exception);
            }
            finally
            {
                work.Progress -= WorkOnProgress;
            }
        }
    }

    private void WorkOnProgress(object sender, EventArgs e)
    {
        var work = (TUnit)sender;
        if (!_activeWorks.Contains(work))
        {
            _activeWorks.Add(work);
        }

        NotifyActiveItemProgress();
    }
}