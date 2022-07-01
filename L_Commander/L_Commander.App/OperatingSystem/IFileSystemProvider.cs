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
    }

    public sealed class FileSystemProvider : IFileSystemProvider
    {
        private readonly IIconCache _iconCache;

        public FileSystemProvider(IIconCache iconCache)
        {
            _iconCache = iconCache;
        }

        public DriveInfo[] GetDrives()
        {
            return DriveInfo.GetDrives();
        }

        public IEnumerable<string> GetFileSystemEntries(string path)
        {
            var entries = Directory.EnumerateFileSystemEntries(path, "*.*", SearchOption.TopDirectoryOnly);
            return entries;
        }

        public FileSystemEntryDescriptor GetEntryDetails(string path)
        {
            var descriptor = new FileSystemEntryDescriptor();

            var attributes = File.GetAttributes(path);
            descriptor.Attributes = attributes;
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                var directoryInfo = new DirectoryInfo(path);
                descriptor.FileOrFolder = FileOrFolder.Folder;
                descriptor.Created = directoryInfo.CreationTime;
                descriptor.Name = directoryInfo.Name;
            }
            else
            {
                descriptor.FileOrFolder = FileOrFolder.File;
                descriptor.Icon = _iconCache.GetByPath(path);

                var fileInfo = new FileInfo(path);

                descriptor.TotalSize = fileInfo.Length;
                descriptor.Extension = fileInfo.Extension;
                descriptor.Created = fileInfo.CreationTime;
                descriptor.Name = fileInfo.Name;
            }

            return descriptor;
        }

        public string GetTopLevelPath(string path)
        {
            var parent = Directory.GetParent(path);

            return parent.FullName;
        }

        public bool IsDriveRoot(string path)
        {
            return Path.GetPathRoot(path) == path;
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetDirectoryName(string path)
        {
            var info = new DirectoryInfo(path);
            return info.Name;
        }
    }
}
