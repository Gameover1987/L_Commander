namespace L_Commander.App.OperatingSystem.Operations;

public class DeleteUnitOfWork : IUnitOfWork
{
    public DeleteUnitOfWork(FileOrFolder fileOrFolder, string path)
    {
        FileOrFolder = fileOrFolder;
        Path = path;
    }

    public FileOrFolder FileOrFolder { get; }

    public string Path { get; }
}