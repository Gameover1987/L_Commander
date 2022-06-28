using System;
using System.Globalization;
using L_Commander.App.FileSystem;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public interface IFileSystemEntryViewModel
{
    string Path { get; }

    bool IsInitialized { get; }

    bool IsFile { get; }

    bool IsSystem { get; }

    bool IsHidden { get; }

    void Initialize();
}

public class FileSystemEntryViewModel : ViewModelBase, IFileSystemEntryViewModel
{
    private readonly string _path;
    private readonly IFileSystemProvider _fileSystemProvider;

    public FileSystemEntryViewModel(string path, IFileSystemProvider fileSystemProvider)
    {
        _path = path;
        _fileSystemProvider = fileSystemProvider;
    }

    public string Path
    {
        get
        {
            if (!IsInitialized)
                InitializeImpl();

            return _path;
        }
    }

    public bool IsFile => FileOrFolder == FileOrFolder.File;

    public bool IsSystem { get; private set; }

    public bool IsHidden { get; private set; }

    public bool IsInitialized { get; private set; }

    public FileOrFolder FileOrFolder { get; private set; }

    public string Extension { get; private set; }

    public string TotalSize { get; private set; }

    public DateTime Created { get; private set; }

    public void Initialize()
    {
        InitializeImpl();
    }

    private void InitializeImpl()
    {
        if (IsInitialized)
            return;

        var descriptor = _fileSystemProvider.GetEntryDetails(_path);

        FileOrFolder = descriptor.FileOrFolder;

        if (descriptor.IsFile)
        {
            Extension = descriptor.Extension;

            var numberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = " " };
            TotalSize = String.Format(numberFormatInfo, "{0:#,#}", descriptor.TotalSize);
        }
        else
        {
            TotalSize = "<DIR>";
        }

        IsHidden = descriptor.IsHidden;
        IsSystem = descriptor.IsSystem;
            
        Created = descriptor.Created;

        IsInitialized = true;

        OnPropertyChanged();
    }
}