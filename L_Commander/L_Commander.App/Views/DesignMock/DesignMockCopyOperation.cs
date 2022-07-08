using System;
using System.Threading.Tasks;
using L_Commander.App.OperatingSystem;
using L_Commander.App.OperatingSystem.Operations;

namespace L_Commander.App.Views.DesignMock;

internal sealed class DesignMockCopyOperation : ICopyOperation
{
    public bool IsStarted => throw new NotImplementedException();

    public event EventHandler<CopyProgressEventArgs> Progress;

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

internal sealed class DesignMockMoveOperation : IMoveOperation
{
    public bool IsStarted => throw new NotImplementedException();

    public event EventHandler<CopyProgressEventArgs> Progress;

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
