using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations;

public interface IUnitOfWork
{

}

public interface IFileSystemOperation<TArgs>
{
    bool IsStarted { get; }

    public event EventHandler<TArgs> Progress;

    Task Execute();

    void Cancel();
}

public abstract class OperationBase<TUnit> : IFileSystemOperation<OperationProgressEventArgs>
    where TUnit : class, IUnitOfWork     
{
    protected readonly ConcurrentQueue<TUnit> _worksQueue = new ConcurrentQueue<TUnit>();
    protected readonly ConcurrentBag<Exception> _errors = new ConcurrentBag<Exception>();

    protected bool _isCancellationRequested;
    protected bool _isInitialized;

    public bool IsStarted { get; protected set; }

    public event EventHandler<OperationProgressEventArgs> Progress;

    public Task Execute()
    {
        if (!_isInitialized)
            throw new ArgumentException($"{GetType().Name} instance is not initialized!");

        var tasks = new List<Task>();
        return Task.Run(() =>
        {
            IsStarted = true;
            try
            {
                PrepareWorksQueue();

                var tasks = new List<Task>();
                for (int i = 0; i < Environment.ProcessorCount / 2; i++)
                {
                    tasks.Add(Task.Run(() => ExecuteThreadMethodWithLogging()));
                }

                Task.WaitAll(tasks.ToArray());

                AfterThreadWorks();
            }
            finally
            {
                IsStarted = false;
            }

            return Task.CompletedTask;
        });
    }

    public void Cancel()
    {
        _isCancellationRequested = true;
    }

    protected abstract void PrepareWorksQueue();

    protected abstract void ThreadMethod();

    protected virtual void AfterThreadWorks() { }

    private void ExecuteThreadMethodWithLogging()
    {
        try
        {
            ThreadMethod();
        }
        catch (Exception exception)
        {
            _errors.Add(exception);
        }
    }
}
