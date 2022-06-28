using System;
using System.IO;

namespace L_Commander.App.OperatingSystem;

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

    public bool IsFile => FileOrFolder == FileOrFolder.File;

    public FileAttributes Attributes { get; set; }

    public bool IsSystem => Attributes.HasFlag(FileAttributes.System);

    public bool IsHidden => Attributes.HasFlag(FileAttributes.Hidden);
}