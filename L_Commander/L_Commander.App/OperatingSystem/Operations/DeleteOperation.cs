using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations
{
    public interface IDeleteOperation : IFileSystemOperation<OperationProgressEventArgs>
    {
        void Initialize(FileSystemEntryDescriptor[] entries);
    }

    public sealed class DeleteOperation : OperationBase<DeleteUnitOfWork>, IDeleteOperation
    {
        private readonly IFileSystemProvider _fileSystemProvider;

        private int _initialCount;

        private FileSystemEntryDescriptor[] _entries;

        public DeleteOperation(IFileSystemProvider fileSystemProvider)
        {
            _fileSystemProvider = fileSystemProvider;
        }

        public event EventHandler<OperationProgressEventArgs> Progress;

        public void Initialize(FileSystemEntryDescriptor[] entries)
        {
            _entries = entries;            
            _isInitialized = true;
            _isCancellationRequested = false;
        }

        protected override void PrepareWorksQueue()
        {            
            foreach(var work in _entries.Select(x => new DeleteUnitOfWork(x.FileOrFolder, x.Path)))
            {
                _worksQueue.Enqueue(work);
            }

            _initialCount = _worksQueue.Count;
        }

        protected override async void ThreadMethod()
        {
            while (!_worksQueue.IsEmpty)
            {
                _worksQueue.TryDequeue(out var work);

                if (work == null)
                    return;

                if (_isCancellationRequested)
                    return;

                NotifyProgress(work);

                _fileSystemProvider.Delete(work.FileOrFolder, work.Path);
            }
        }

        private void NotifyProgress(DeleteUnitOfWork work)
        {
            Progress?.Invoke(this, new OperationProgressEventArgs
            {
                Processed = _initialCount - _worksQueue.Count,
                Total = _initialCount,
                CurrentItemName = work.Path
            });
        }
    }
}