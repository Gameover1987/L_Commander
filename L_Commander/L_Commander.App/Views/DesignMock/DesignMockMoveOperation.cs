using System;
using System.Threading.Tasks;
using L_Commander.App.OperatingSystem;
using L_Commander.App.OperatingSystem.Operations;

namespace L_Commander.App.Views.DesignMock;

internal sealed class DesignMockMoveOperation : IMoveOperation
{
    public bool IsStarted => throw new NotImplementedException();
    public bool HasErrors { get; }
    public Exception[] Errors { get; }

    public event EventHandler<OperationProgressEventArgs> TotalProgress;
    public event EventHandler<CopyProgressEventArgs> ActiveItemsProgress;

    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public Task Execute()
    {
        throw new NotImplementedException();
    }

    public void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory)
    {
        throw new NotImplementedException();
    }
}