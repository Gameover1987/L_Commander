namespace L_Commander.App.OperatingSystem.Operations;

public class DeleteUnitOfWork : IUnitOfWork
{
    public DeleteUnitOfWork(FileOrFolder fileOrFolder, string path)
    {
        FileOrFolder = fileOrFolder;
        SourcePath = path;
    }

    public FileOrFolder FileOrFolder { get; }

    public string SourcePath { get; }
}