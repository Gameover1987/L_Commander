namespace L_Commander.App.OperatingSystem.Operations
{
    public interface IDeleteOperation : IFileSystemOperation
    {
        void Initialize(FileSystemEntryDescriptor[] entries);
    }
}