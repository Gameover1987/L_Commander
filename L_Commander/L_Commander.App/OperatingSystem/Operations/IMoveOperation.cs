namespace L_Commander.App.OperatingSystem.Operations
{
    public interface IMoveOperation : IFileSystemOperation<OperationProgressEventArgs>
    {
        void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory);
    }
}