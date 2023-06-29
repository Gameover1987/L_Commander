using System;

namespace L_Commander.App.OperatingSystem.Operations;

public class CopyProgressEventArgs : EventArgs
{
    public CopyProgressEventArgs(CopyUnitOfWork[] activeWorks)
    {
        ActiveWorks = activeWorks;
    }

    public CopyUnitOfWork[] ActiveWorks { get; }
}