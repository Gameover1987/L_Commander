using System;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations
{
    public interface ICopyOperation : IFileSystemOperation
    {
        event EventHandler<CopyProgressEventArgs> ActiveItemsProgress;

        void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory);
    }
}
