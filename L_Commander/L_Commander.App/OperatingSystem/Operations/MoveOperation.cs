﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations
{
    public interface IMoveOperation : IFileSystemOperation<CopyProgressEventArgs>
    {
        void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory);
    }

    public sealed class MoveOperation : OperationBase<CopyUnitOfWork, CopyProgressEventArgs>, IMoveOperation
    {
        private readonly IFileSystemProvider _fileSystemProvider;

        private int _initialCount;        

        private FileSystemEntryDescriptor[] _entries;
        private string _destDirectory;

        public MoveOperation(IFileSystemProvider fileSystemProvider)
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

        public event EventHandler<CopyProgressEventArgs> Progress;

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
                _works.Enqueue(work);
            }

            _initialCount = _works.Count + _entries.Length;
        }

        protected override void ThreadMethod()
        {
            while (!_works.IsEmpty)
            {
                _works.TryDequeue(out var work);

                if (work == null)
                    return;

                if (_isCancellationRequested)
                    return;

                Progress?.Invoke(this, new CopyProgressEventArgs
                {
                    Copied = _initialCount - _entries.Length - _works.Count,
                    Total = _initialCount,
                    CurrentFileName = work.SourcePath
                });

                _fileSystemProvider.Move(work.SourcePath, work.DestinationPath);
            }
        }

        protected override void AfterThreadWorks()
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                Progress?.Invoke(this, new CopyProgressEventArgs
                {
                    Copied = _initialCount - (_entries.Length - i),
                    Total = _initialCount,
                    CurrentFileName = _entries[i].Path
                });
                _fileSystemProvider.Delete(_entries[i].FileOrFolder, _entries[i].Path);
            }
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
}
