﻿using System.Collections.Generic;
using System.IO;

namespace L_Commander.App.FileSystem
{
    public interface IFileSystemProvider
    {
       IEnumerable<string> GetFileSystemEntries(string path);

       FileSystemEntryDescriptor GetEntryDetails(string path);

       string GetTopLevelPath(string path);

       bool IsDriveRoot(string path);
    }

    public sealed class FileSystemProvider : IFileSystemProvider
    {
        public IEnumerable<string> GetFileSystemEntries(string path)
        {
            var entries = Directory.EnumerateFileSystemEntries(path, "*.*", SearchOption.TopDirectoryOnly);
            return entries;
        }

        public FileSystemEntryDescriptor GetEntryDetails(string path)
        {
            var descriptor = new FileSystemEntryDescriptor();

            var attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                descriptor.FileOrFolder = FileOrFolder.Folder;
                descriptor.Created = Directory.GetCreationTime(path);
            }
            else
            {
                descriptor.FileOrFolder = FileOrFolder.File;

                var fileInfo = new FileInfo(path);
                
                descriptor.TotalSize = fileInfo.Length;
                descriptor.Extension = fileInfo.Extension;
                descriptor.Created = fileInfo.CreationTime;
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
    }
}
