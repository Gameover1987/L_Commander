using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using L_Commander.Common.Extensions;

namespace L_Commander.App.OperatingSystem.Operations;

public sealed class CopyOperation : OperationBase<CopyUnitOfWork>, ICopyOperation
{
    private readonly IFileSystemProvider _fileSystemProvider;    

    private int _initialCount;    

    private FileSystemEntryDescriptor[] _entries;
    private string _destDirectory;

    public CopyOperation(IFileSystemProvider fileSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;
    }

    public void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory)
    {
        _entries = entries;
        _destDirectory = destDirectory;
        _isInitialized = true;
        _isCancellationRequested = false;
    }

    public event EventHandler<OperationProgressEventArgs> Progress;

    protected override void PrepareWorksQueue()
    {
        var works = GetUnitOfWorks(_entries, _destDirectory);
        var folders = works.Select(x => _fileSystemProvider.GetTopLevelPath(x.DestinationPath)).Distinct().ToArray();
        foreach (var folder in folders)
        {
            if (!_fileSystemProvider.IsDirectoryExists(folder))
            {
                _fileSystemProvider.CreateDirectory(folder);
            }
        }

        foreach (var work in works)
        {
            _worksQueue.Enqueue(work);
        }

        _initialCount = _worksQueue.Count;
    }

    protected override void ThreadMethod()
    {
        while (!_worksQueue.IsEmpty)
        {
            _worksQueue.TryDequeue(out var work);

            if (work == null)
                return;

            if (_isCancellationRequested)
                return;

            NotifyProgress(work);

            _fileSystemProvider.Copy(work.SourcePath, work.DestinationPath);
        }
    }

    private void NotifyProgress(ICopyUnitOfWork work)
    {
        Progress?.Invoke(this, new OperationProgressEventArgs
        {
            Processed = _initialCount - _worksQueue.Count,
            Total = _initialCount,
            CurrentItemName = work.SourcePath
        });
    }

    private CopyUnitOfWork[] GetUnitOfWorks(FileSystemEntryDescriptor[] entries, string destDirectory)
    {
        var units = new List<CopyUnitOfWork>();
        foreach (var descriptor in entries)
        {
            if (descriptor.IsFile)
            {
                units.Add(new CopyUnitOfWork(_fileSystemProvider.GetTopLevelPath(descriptor.Path), descriptor.Path, destDirectory));
            }
            else
            {
                var files = _fileSystemProvider.GetFilesRecursively(descriptor.Path);
                units.AddRange(files.Select(x =>
                    new CopyUnitOfWork(_fileSystemProvider.GetTopLevelPath(descriptor.Path), x, destDirectory)));
            }
        }

        return units.ToArray();
    }
}