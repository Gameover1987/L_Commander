using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace L_Commander.App.OperatingSystem;

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

    public IEnumerable<string> GetFilesRecursively(string path)
    {
        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
        return files;
    }

    public FileSystemEntryDescriptor GetEntryDetails(string path)
    {
        var descriptor = new FileSystemEntryDescriptor();
        descriptor.Path = path;

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
            descriptor.Name = Path.GetFileNameWithoutExtension(fileInfo.Name);
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

    public void Rename(FileOrFolder fileOrFolder, string oldName, string newName)
    {
        if (fileOrFolder == FileOrFolder.File)
        {
            var fileInfo = new FileInfo(oldName);
            var newFullPath = Path.Combine(Path.GetDirectoryName(oldName), newName + fileInfo.Extension);
            fileInfo.MoveTo(newFullPath);
        }
        else
        {
            var directoryInfo = new DirectoryInfo(oldName);
            var newFullPath = Path.Combine(Path.GetDirectoryName(oldName), newName);
            directoryInfo.MoveTo(newFullPath);
        }
    }

    public void Delete(FileOrFolder fileOrFolder, string path)
    {
        if (fileOrFolder == FileOrFolder.File)
        {
            var fileInfo = new FileInfo(path);
            fileInfo.Delete();
        }
        else
        {
            var directoryInfo = new DirectoryInfo(path);
            directoryInfo.Delete(true);
        }
    }

    public void Copy(string sourcePath, string destinationPath)
    {
        var sourceFile = new FileInfo(sourcePath);
        var destinationFile = new FileInfo(destinationPath);
        if (!destinationFile.Directory.Exists)
        {
            destinationFile.Directory.Create();
        }

        sourceFile.CopyTo(destinationPath);
    }

    public void MakeDirectory(string path, string folderName)
    {
        var directoryPath = Path.Combine(path, folderName);
        Directory.CreateDirectory(directoryPath);
    }

    public bool IsDirectoryExists(string path)
    {
        return Directory.Exists(path);
    }
}