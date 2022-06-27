using System;
using System.Globalization;
using L_Commander.App.FileSystem;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public interface IFileSystemEntryViewModel
{
    string Path { get; }

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

    public string Path => _path;

    public bool IsInitialized { get; private set; }

    public FileOrFolder FileOrFolder { get; private set; }

    public string Extension { get; private set; }

    public string TotalSize { get; private set; }

    public DateTime Created { get; private set; }

    public void Initialize()
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
            
        Created = descriptor.Created;

        IsInitialized = true;

        OnPropertyChanged();
    }
}