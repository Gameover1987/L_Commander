using System;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations;

public interface IFileSystemOperation
{
    bool IsStarted { get; }

    bool HasErrors { get; }

    Exception[] Errors { get; }

    public event EventHandler<OperationProgressEventArgs> TotalProgress;

    Task Execute();

    void Cancel();
}
