using System;
using System.Threading;

namespace L_Commander.App.OperatingSystem.Operations;

public interface IUnitOfWork
{
    public string SourcePath { get; }

    public event EventHandler Progress;

    public void Do(CancellationToken cancellationToken);
}
