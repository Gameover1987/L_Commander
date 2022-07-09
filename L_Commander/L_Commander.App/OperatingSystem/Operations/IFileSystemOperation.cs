using System;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations;

public interface IFileSystemOperation<TArgs>
{
    bool IsStarted { get; }

    public event EventHandler<TArgs> Progress;

    Task Execute();

    void Cancel();
}
