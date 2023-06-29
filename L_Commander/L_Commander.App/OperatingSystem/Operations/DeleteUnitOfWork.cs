using System;
using System.IO;
using System.Threading;

namespace L_Commander.App.OperatingSystem.Operations;

public class DeleteUnitOfWork : IUnitOfWork
{
    private readonly IFileSystemProvider _fileSystemProvider;

    public DeleteUnitOfWork(IFileSystemProvider fileSystemProvider, FileOrFolder fileOrFolder, string path)
    {
        _fileSystemProvider = fileSystemProvider;
        FileOrFolder = fileOrFolder;
        SourcePath = path;
    }

    public FileOrFolder FileOrFolder { get; }

    public string SourcePath { get; }


    public event EventHandler Progress;

    public void Do(CancellationToken cancellationToken)
    {
        _fileSystemProvider.Delete(FileOrFolder, SourcePath);
    }
}