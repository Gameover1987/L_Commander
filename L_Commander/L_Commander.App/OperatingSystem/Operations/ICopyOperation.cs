using System;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations
{
    public interface ICopyOperationEventArgs : IOperationEventArgs
    {
        public long Copied { get; set; }

        public long Total { get; set; }

        public string CurrentFileName { get; set; }
    }

    public class CopyProgressEventArgs : EventArgs, ICopyOperationEventArgs
    {
        public long Copied { get; set; }

        public long Total { get; set; }

        public string CurrentFileName { get; set; }

    }

    public class MoveProgressEventArgs : EventArgs, ICopyOperationEventArgs
    {
        public long Copied { get; set; }

        public long Total { get; set; }

        public string CurrentFileName { get; set; }
    }

    public interface ICopyOperation : IFileSystemOperation<CopyProgressEventArgs>
    {
        void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory);
    }
}
