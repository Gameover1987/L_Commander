namespace L_Commander.App.OperatingSystem.Operations
{
    public interface IDeleteOperation : IFileSystemOperation<OperationProgressEventArgs>
    {
        void Initialize(FileSystemEntryDescriptor[] entries);
    }
}