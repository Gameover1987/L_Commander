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

        var tasks = new List<Task>();
        return Task.Run(async () =>
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

            var tasks = new List<Task>();
            for (int i = 0; i < Environment.ProcessorCount / 2; i++)
            {
                tasks.Add(Task.Run(() => ThreadMethod($"CopyTask_({i})")));
            }

            Task.WaitAll(tasks.ToArray());
        });
    }

    public void Cancel()
    {
        _isCancellationRequested = true;
    }

    public event EventHandler<CopyProgressEventArgs> Progress;

    private void ThreadMethod(string methodName)
    {
        while (!_worksQueue.IsEmpty)
        {
            _worksQueue.TryDequeue(out var work);

            if (work == null)
                return;

            if (_isCancellationRequested)
                return;

            Progress?.Invoke(this, new CopyProgressEventArgs { Copied = _initialCount - _worksQueue.Count, Total = _initialCount, CurrentFileName = work.SourcePath });
            _fileSystemProvider.Copy(work.SourcePath, work.DestinationPath);
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