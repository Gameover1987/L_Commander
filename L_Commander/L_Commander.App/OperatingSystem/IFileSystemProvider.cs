using System;
using System.Collections.Generic;
using System.IO;

namespace L_Commander.App.OperatingSystem
{
    public interface IFileSystemProvider
    {
        DriveInfo[] GetDrives();

        IEnumerable<string> GetFileSystemEntries(string path);

        IEnumerable<string> GetFilesRecursively(string path);

        FileSystemEntryDescriptor GetFileSystemDescriptor(string path);

        string GetTopLevelPath(string path);

        bool IsDriveRoot(string path);

        string GetFileName(string path);

        string GetFileExtension(string path);

        string GetDirectoryName(string path);

        void Rename(FileOrFolder fileOrFolder, string oldName, string newName);

        void Delete(FileOrFolder fileOrFolder, string path);

        void Copy(string sourcePath, string destinationPath);

        void Move(string sourcePath, string destinationPath);

        void CreateDirectory(string path);        

        bool IsDirectoryExists(string path);

        bool IsFileExists(string file);

        string CombinePaths(params string[] paths);

        long CalculateFolderSize(string folderPath);

        void Initialize();

        event EventHandler DrivesChanged;
    }
}
