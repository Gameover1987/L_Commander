using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure.History;

namespace L_Commander.App.OperatingSystem.Operations;

public abstract class OperationBase<TUnit> : IFileSystemOperation<OperationProgressEventArgs>
    where TUnit : class, IUnitOfWork     
{
    protected readonly ConcurrentQueue<TUnit> _worksQueue = new ConcurrentQueue<TUnit>();
    protected readonly ConcurrentBag<Exception> _errors = new ConcurrentBag<Exception>();

    private CancellationTokenSource _canellationTokenSource;    
    protected bool _isInitialized;

    public bool IsStarted { get; protected set; }

    public event EventHandler<OperationProgressEventArgs> Progress;

    public Task Execute()
    {
        if (!_isInitialized)
            throw new ArgumentException($"{GetType().Name} instance is not initialized!");

        if (_canellationTokenSource != null)
        {
            _canellationTokenSource.Cancel();
            _canellationTokenSource.Dispose();
            _canellationTokenSource = null;
        }      

        _canellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _canellationTokenSource.Token;

        var tasks = new List<Task>();
        return Task.Run(() =>
        {
            IsStarted = true;
            try
            {
                Setup();

                var tasks = new List<Task>();
                for (int i = 0; i < Environment.ProcessorCount / 2; i++)
                {
                    tasks.Add(Task.Run(() => ExecuteThreadMethodWithLogging(cancellationToken), cancellationToken));
                }

                Task.WaitAll(tasks.ToArray());

                Cleanup();
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
        _canellationTokenSource?.Cancel();

        _worksQueue.Clear();
    }

    protected abstract void Setup();

    protected abstract void ThreadMethod(TUnit unitOfWork);

    protected virtual void Cleanup()
    {

    }

    protected abstract OperationProgressEventArgs GetProgressEventArgs(TUnit unitOfWork);

    protected void NotifyProgress(TUnit unitOfWork)
    {
        Progress?.Invoke(this, GetProgressEventArgs(unitOfWork));
    }
    private void ExecuteThreadMethodWithLogging(CancellationToken cancellationToken)
    {
        try
        {
            while (!_worksQueue.IsEmpty)
            {
                _worksQueue.TryDequeue(out var work);

                if (work == null)
                    return;

                if (cancellationToken.IsCancellationRequested)
                    return;

                NotifyProgress(work);

                ThreadMethod(work);
            }
        }
        catch (Exception exception)
        {
            _errors.Add(exception);
        }
    }
}
