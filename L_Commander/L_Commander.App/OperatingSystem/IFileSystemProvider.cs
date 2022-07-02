using System.Collections.Generic;
using System.IO;

namespace L_Commander.App.OperatingSystem
{
    public interface IFileSystemProvider
    {
        DriveInfo[] GetDrives();

        IEnumerable<string> GetFileSystemEntries(string path);

        FileSystemEntryDescriptor GetEntryDetails(string path);

        string GetTopLevelPath(string path);

        bool IsDriveRoot(string path);

        string GetFileName(string path);

        string GetDirectoryName(string path);

        void Rename(FileOrFolder fileOrFolder, string oldName, string newName);

        void Delete(FileOrFolder fileOrFolder, string path);
    }
}
