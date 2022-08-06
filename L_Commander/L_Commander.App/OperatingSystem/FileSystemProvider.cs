using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using L_Commander.App.OperatingSystem.Operations;

namespace L_Commander.App.OperatingSystem;

public sealed class FileSystemProvider : IFileSystemProvider
{
    private const int WM_DEVICECHANGE = 0x0219;

    private readonly IIconCache _iconCache;

    public FileSystemProvider(IIconCache iconCache)
    {
        _iconCache = iconCache;
    }

    public DriveInfo[] GetDrives()
    {
        return DriveInfo.GetDrives();
    }

    public IEnumerable<FileSystemEntryDescriptor> GetFileSystemEntries(string path)
    {
        var directory = new DirectoryInfo(path);
        var directories = directory
            .EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly)
            .Select(x => new FileSystemEntryDescriptor
            {
                Path = x.FullName,
                Created = x.CreationTime,
                FileOrFolder = FileOrFolder.Folder,
                Name = x.Name
            });

        var files = directory
            .EnumerateFiles("*.*", SearchOption.TopDirectoryOnly)
            .Select(x => new FileSystemEntryDescriptor
            {
                Path = x.FullName,
                Created = x.CreationTime,
                Extension = x.Extension,
                FileOrFolder = FileOrFolder.File,
                Icon = _iconCache.GetByPath(x.FullName),
                Name = x.Name,
                TotalSize = x.Length,
            });

        return directories.Concat(files);
    }

    public IEnumerable<string> GetFilesRecursively(string path)
    {
        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
        return files;
    }

    public FileSystemEntryDescriptor GetFileSystemDescriptor(string path)
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

    public FileSystemEntryDescriptor[] GetPathByParts(string path)
    {
        var directory = new DirectoryInfo(path);
        var parts = new List<FileSystemEntryDescriptor>();

        do
        {
            parts.Add(GetFileSystemDescriptor(directory.FullName));
            directory = directory.Parent;
        } while (directory != null);

        parts.Reverse();
        return parts.ToArray();
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

    public string GetFileExtension(string path)
    {
        return Path.GetExtension(path);
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
            if (fileInfo.Exists)
                fileInfo.Delete();
        }
        else
        {
            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
                directoryInfo.Delete(true);
        }
    }

    public void Copy(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        CopyImpl(sourcePath, destinationPath, cancellationToken);
    }

    private void CopyImpl(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        var fromFile = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
        var toFile = new FileStream(destinationPath, FileMode.Append, FileAccess.Write);

        var bufferSize = 1024 * 1024 * 5;

        if (bufferSize < fromFile.Length)
        {
            var buffer = new byte[bufferSize];
            long copied = 0;
            while (copied <= fromFile.Length - bufferSize)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    fromFile.Close();
                    toFile.Close();
                    File.Delete(destinationPath);
                    return;
                }

                var toCopyLength = fromFile.Read(buffer, 0, bufferSize);
                fromFile.Flush();
                toFile.Write(buffer, 0, bufferSize);
                toFile.Flush();

                toFile.Position = fromFile.Position;
                copied += toCopyLength;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                fromFile.Close();
                toFile.Close();
                File.Delete(destinationPath);
                return;
            }

            var left = (int)(fromFile.Length - copied);
            fromFile.Read(buffer, 0, left);
            fromFile.Flush();
            toFile.Write(buffer, 0, left);
            toFile.Flush();
        }
        else
        {
            var buffer = new byte[fromFile.Length];
            fromFile.Read(buffer, 0, buffer.Length);
            fromFile.Flush();
            toFile.Write(buffer, 0, buffer.Length);
            toFile.Flush();

        }
        fromFile.Close();
        toFile.Close();
    }

    public void Move(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
       CopyImpl(sourcePath, destinationPath, cancellationToken);

       if (cancellationToken.IsCancellationRequested)
           return;

       File.Delete(sourcePath);
    }

    public bool IsDirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public string CombinePaths(params string[] paths)
    {
        return Path.Combine(paths);
    }

    public PathInfo GetPathInfoRecursively(string folderPath)
    {
        var dirInfo = new DirectoryInfo(folderPath);
        var enumeratedFiles = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).ToArray();
        var enumeratedDirs = dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories);

        return new PathInfo
        {
            FilesCount = enumeratedFiles.Count(),
            FoldersCount = enumeratedDirs.Count(),
            TotalSize = enumeratedFiles.Select(x => x.Length).Sum()
        };
    }

    public bool IsFileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public void Initialize()
    {
        if (Application.Current?.MainWindow == null)
            return;

        var source = PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource;
        source?.AddHook(WndProc);
    }

    public event EventHandler DrivesChanged;

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_DEVICECHANGE)
            return IntPtr.Zero;

        DrivesChanged?.Invoke(this, EventArgs.Empty);
        return IntPtr.Zero;
    }
}