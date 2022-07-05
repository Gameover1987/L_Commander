using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L_Commander.Common.Extensions;

namespace L_Commander.App.OperatingSystem.Operations;

public sealed class CopyOperation : ICopyOperation
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private bool _isCancellationRequested;

    public CopyOperation(IFileSystemProvider fileSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;
    }

    public bool IsBusy
    {
        get { return false; }
    }

    public Task Execute(FileSystemEntryDescriptor[] entries, string destDirectory)
    {
        _isCancellationRequested = false;
        return ThreadTaskExtensions.Run(() =>
        {
            var works = GetUnitOfWorks(entries, destDirectory);
            var aaa = works.Select(x => x.DestinationPath).ToArray();
            for (int i = 0; i < works.Length; i++)
            {
                if (_isCancellationRequested)
                    return;

                Progress?.Invoke(this, new CopyProgressEventArgs { Copied = i, Total = works.Length, CurrentFileName = works[i].SourcePath });
                _fileSystemProvider.Copy(works[i].SourcePath, works[i].DestinationPath);
            }
        });
    }

    public void Cancel()
    {
        _isCancellationRequested = true;
    }

    public event EventHandler<CopyProgressEventArgs> Progress;

    private UnitOfWork[] GetUnitOfWorks(FileSystemEntryDescriptor[] entries, string destDirectory)
    {
        var units = new List<UnitOfWork>();
        foreach (var descriptor in entries)
        {
            if (descriptor.IsFile)
            {
                units.Add(new UnitOfWork(_fileSystemProvider.GetTopLevelPath(descriptor.Path), descriptor.Path, destDirectory));
            }
            else
            {
                var files = _fileSystemProvider.GetFilesRecursively(descriptor.Path);
                units.AddRange(files.Select(x =>
                    new UnitOfWork(_fileSystemProvider.GetTopLevelPath(descriptor.Path), x, destDirectory)));
            }
        }

        return units.ToArray();
    }
}