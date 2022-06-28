using System;

namespace L_Commander.App.FileSystem;

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
}