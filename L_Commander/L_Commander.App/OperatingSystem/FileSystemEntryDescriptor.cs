using System;
using System.IO;
using System.Windows.Media;

namespace L_Commander.App.OperatingSystem;

public enum FileOrFolder
{
    Folder = 0,
    File = 1
}

public class FileSystemEntryDescriptor
{
    public FileOrFolder FileOrFolder { get; set; }

    public ImageSource Icon { get; set; }

    public string Path { get; set; }

    public string Extension { get; set; }

    public long TotalSize { get; set; }

    public DateTime Created { get; set; }

    public bool IsFile => FileOrFolder == FileOrFolder.File;

    public FileAttributes Attributes { get; set; }

    public bool IsSystem => Attributes.HasFlag(FileAttributes.System);

    public bool IsHidden => Attributes.HasFlag(FileAttributes.Hidden);
    public string Name { get; set; }
}