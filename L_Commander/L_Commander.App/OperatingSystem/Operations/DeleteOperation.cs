using System.Linq;
using System.Threading;

namespace L_Commander.App.OperatingSystem.Operations
{
    public sealed class DeleteOperation : OperationBase<DeleteUnitOfWork>, IDeleteOperation
    {
        private readonly IFileSystemProvider _fileSystemProvider;

        private int _initialCount;

        private FileSystemEntryDescriptor[] _entries;

        public DeleteOperation(IFileSystemProvider fileSystemProvider)
        {
            _fileSystemProvider = fileSystemProvider;
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
        }

        protected override void ThreadMethod(DeleteUnitOfWork unitOfWork)
        {
            _fileSystemProvider.Delete(unitOfWork.FileOrFolder, unitOfWork.SourcePath);
        }
    }
}