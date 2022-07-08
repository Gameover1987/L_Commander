using System;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations
{
    public class OperationProgressEventArgs : EventArgs
    {
        public long Processed { get; set; }

        public long Total { get; set; }

        public string CurrentItemName { get; set; }

    }

    public interface ICopyOperation : IFileSystemOperation<OperationProgressEventArgs>
    {
        void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory);
    }
}
