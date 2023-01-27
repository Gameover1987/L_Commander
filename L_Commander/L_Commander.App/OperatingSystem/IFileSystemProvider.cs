using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static System.Environment;

namespace L_Commander.App.OperatingSystem
{

    public interface IFileSystemProvider
    {
        DriveInfo[] GetDrives();

        IEnumerable<FileSystemEntryDescriptor> GetFileSystemEntries(string path);

        IEnumerable<string> GetFilesRecursively(string path);

        FileSystemEntryDescriptor GetFileSystemDescriptor(string path);

        FileSystemEntryDescriptor[] GetPathByParts(string path);

        string GetTopLevelPath(string path);

        bool IsDriveRoot(string path);

        string GetFileName(string path);

        string GetFileExtension(string path);

        string GetDirectoryName(string path);

        void Rename(FileOrFolder fileOrFolder, string oldName, string newName);

        void Delete(FileOrFolder fileOrFolder, string path);

        void Copy(string sourcePath, string destinationPath, CancellationToken cancellationToken);

        void Move(string sourcePath, string destinationPath, CancellationToken cancellationToken);

        void CreateDirectory(string path);        

        bool IsDirectoryExists(string path);

        bool IsFileExists(string file);

        string CombinePaths(params string[] paths);

        PathInfo GetPathInfoRecursively(string folderPath);

        string GetSpecialFolderPath(SpecialFolder specialFolder);

        void Initialize();

        event EventHandler DrivesChanged;
    }
}
