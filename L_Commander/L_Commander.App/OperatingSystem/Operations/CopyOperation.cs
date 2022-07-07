using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using L_Commander.Common.Extensions;

namespace L_Commander.App.OperatingSystem.Operations;

public sealed class CopyOperation : ICopyOperation
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private bool _isCancellationRequested;

    private readonly ConcurrentQueue<UnitOfWork> _worksQueue = new ConcurrentQueue<UnitOfWork>();
    private int _initialCount;
    private int _entriesCount;

    public CopyOperation(IFileSystemProvider fileSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;
    }

    public bool IsBusy { get; private set; }

    public Task Execute(FileSystemEntryDescriptor[] entries, string destDirectory, bool cleanupSourceEntries = false)
    {
        _isCancellationRequested = false;

        var tasks = new List<Task>();
        return Task.Run(async () =>
        {
            IsBusy = true;
            try
            {
                var works = GetUnitOfWorks(entries, destDirectory);
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
                if (cleanupSourceEntries)
                {
                    _entriesCount = entries.Length;
                    _initialCount += _entriesCount;
                }

                var tasks = new List<Task>();
                for (int i = 0; i < Environment.ProcessorCount / 2; i++)
                {
                    tasks.Add(Task.Run(() => ThreadMethod(cleanupSourceEntries)));
                }

                Task.WaitAll(tasks.ToArray());

                if (cleanupSourceEntries)
                {
                    for (int i = 0; i < entries.Length; i++)
                    {
                        Progress?.Invoke(this, new CopyProgressEventArgs
                        {
                            Copied = _initialCount - (entries.Length - i),
                            Total = _initialCount,
                            CurrentFileName = entries[i].Path
                        });
                        _fileSystemProvider.Delete(entries[i].FileOrFolder, entries[i].Path);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        });
    }

    public void Cancel()
    {
        _isCancellationRequested = true;
    }

    public event EventHandler<CopyProgressEventArgs> Progress;

    private void ThreadMethod(bool isMoving)
    {
        while (!_worksQueue.IsEmpty)
        {
            _worksQueue.TryDequeue(out var work);

            if (work == null)
                return;

            if (_isCancellationRequested)
                return;

            Progress?.Invoke(this, new CopyProgressEventArgs
            {
                Copied = isMoving == true ?
                    _initialCount - _entriesCount - _worksQueue.Count :
                    _initialCount - _worksQueue.Count,
                Total = _initialCount,
                CurrentFileName = work.SourcePath
            });
            if (isMoving)
            {
                _fileSystemProvider.Move(work.SourcePath, work.DestinationPath);
            }
            else
            {
                _fileSystemProvider.Copy(work.SourcePath, work.DestinationPath);
            }
        }
    }

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