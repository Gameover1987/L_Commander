using System;
using System.IO;

namespace L_Commander.App.OperatingSystem.Operations;

public class UnitOfWork
{
    private readonly string _sourceRoot;
    private readonly string _destinationDirectory;

    public UnitOfWork(string sourceRoot, string sourcePath, string destinationDirectory)
    {
        _sourceRoot = sourceRoot;
        _destinationDirectory = destinationDirectory;

        SourcePath = sourcePath;

        var sourceWithoutRoot = sourcePath.Substring(sourceRoot.Length + 1);
        DestinationPath = Path.Combine(destinationDirectory, sourceWithoutRoot);
    }

    public string SourcePath { get; }

    public string DestinationPath { get; }
}