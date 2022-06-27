﻿using System;
using System.Collections.Generic;
using System.IO;

namespace L_Commander.App.FileSystem
{
    public enum FileOrFolder
    {
        File, Folder
    }

    public class FileSystemEntryDescriptor
    {
        public FileOrFolder FileOrFolder { get; set; }

        public string Path { get; set; }

        public string Extension { get; set; }

        public long TotalSize { get; set; }

        public DateTime Created { get; set; }

        public bool IsFile
        {
            get { return FileOrFolder == FileOrFolder.File; }
        }
    }

    public interface IFileSystemProvider
    {
       IEnumerable<string> GetFileSystemEntries(string path);

       FileSystemEntryDescriptor GetEntryDetails(string path);
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
    }
}