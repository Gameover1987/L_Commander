using System;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations
{
    public class CopyProgressEventArgs : EventArgs
    {
        public long Copied { get; set; }

        public long Total { get; set; }

        public string CurrentFileName { get; set; }

    }

    public interface ICopyOperation
    {
        bool IsBusy { get; }

        Task Execute(FileSystemEntryDescriptor[] entries, string destDirectory, bool cleanupSourceEntries = false);

        void Cancel();

        event EventHandler<CopyProgressEventArgs> Progress;
    }
}
