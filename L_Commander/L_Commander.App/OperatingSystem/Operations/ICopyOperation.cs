using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem.Operations
{

    public interface ICopyOperation : IFileSystemOperation<OperationProgressEventArgs>
    {
        void Initialize(FileSystemEntryDescriptor[] entries, string destDirectory);
    }
}
