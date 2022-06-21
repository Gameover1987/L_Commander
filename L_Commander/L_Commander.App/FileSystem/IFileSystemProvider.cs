using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.FileSystem
{
    public enum FileOrFolder
    {
        File, Folder
    }

    public class FileSystemEntry
    {
        private readonly string _path;

        public FileSystemEntry(string path)
        {
            _path = path;

            var attributes = File.GetAttributes(_path);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                FileOrFolder = FileOrFolder.Folder;
                TotalSize = "<DIR>";
                Created = Directory.GetCreationTime(_path);
            }
            else
            {
                FileOrFolder = FileOrFolder.File;
                Extension = System.IO.Path.GetExtension(_path);
                Created = File.GetCreationTime(_path);
            }
        }

        public FileOrFolder FileOrFolder { get; set; }

        public string Path => _path;

        public string Extension { get; private set; }

        public string TotalSize { get; private set; }

        public DateTime Created { get; private set; }
    }

    public interface IFileSystemProvider
    {
       IEnumerable<FileSystemEntry> GetFileSystemEntries(string path);
    }

    public sealed class FileSystemProvider : IFileSystemProvider
    {
        public IEnumerable<FileSystemEntry> GetFileSystemEntries(string path)
        {
            var entries = Directory.EnumerateFileSystemEntries(path, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var entry in entries)
            {
                yield return new FileSystemEntry(entry);
            }
        }
    }
}
