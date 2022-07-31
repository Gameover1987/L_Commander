using System;
using System.Linq;
using System.Threading;
using L_Commander.App.Infrastructure.History;

namespace L_Commander.App.OperatingSystem.Operations
{
    public sealed class DeleteOperation : OperationBase<DeleteUnitOfWork>, IDeleteOperation
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IHistoryManager _historyManager;

        private int _initialCount;

        private FileSystemEntryDescriptor[] _entries;

        public DeleteOperation(IFileSystemProvider fileSystemProvider, IHistoryManager historyManager)
        {
            _fileSystemProvider = fileSystemProvider;
            _historyManager = historyManager;
        }

        public void Initialize(FileSystemEntryDescriptor[] entries)
        {
            _entries = entries;
            _isInitialized = true;
        }

        protected override OperationProgressEventArgs GetProgressEventArgs(DeleteUnitOfWork unitOfWork)
        {
            return new OperationProgressEventArgs
            {
                Processed = _initialCount - _entries.Length - _worksQueue.Count,
                Total = _initialCount,
                CurrentItemName = unitOfWork.SourcePath
            };
        }

        protected override void Setup()
        {
            _historyManager.Add("Delete operation started", string.Join("", _entries.Select(x => x.Path + Environment.NewLine)));

            foreach (var work in _entries.Select(x => new DeleteUnitOfWork(x.FileOrFolder, x.Path)))
            {
                _worksQueue.Enqueue(work);
            }

            _initialCount = _worksQueue.Count + _entries.Length;
        }

        protected override void Cleanup()
        {
            foreach (var descriptor in _entries)
            {
                _fileSystemProvider.Delete(descriptor.FileOrFolder, descriptor.Path);
            }

            _historyManager.Add("Delete operation finished succesfully", string.Join("", _entries.Select(x => x.Path + Environment.NewLine)));
        }

        protected override void ThreadMethod(DeleteUnitOfWork unitOfWork)
        {
            _fileSystemProvider.Delete(unitOfWork.FileOrFolder, unitOfWork.SourcePath);
        }
    }
}