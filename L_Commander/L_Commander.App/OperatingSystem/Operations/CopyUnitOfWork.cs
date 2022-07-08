using System;
using System.IO;

namespace L_Commander.App.OperatingSystem.Operations;

public interface ICopyUnitOfWork : IUnitOfWork
{
    string SourcePath { get; }
}

public class CopyUnitOfWork : ICopyUnitOfWork
{
    public CopyUnitOfWork(string sourceRoot, string sourcePath, string destinationDirectory)
    {
        SourcePath = sourcePath;

        var sourceWithoutRoot = sourcePath.Substring(sourceRoot.Length + 1);
        DestinationPath = destinationDirectory + "\\" + sourceWithoutRoot;
    }

    public string SourcePath { get; }

    public string DestinationPath { get; }
}