using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using L_Commander.App.FileSystem;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileManagerTabViewModel : ViewModelBase, IFileManagerTabViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private string _path;

    public FileManagerTabViewModel(IFileSystemProvider fileSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;
    }

    public string Path
    {
        get { return _path; }
    }

    public ObservableCollection<FileSystemEntry> FileSystemEntries { get; } = new ObservableCollection<FileSystemEntry>();

    public void SetPath(string path)
    {
        _path = path;

        var entries = _fileSystemProvider.GetFileSystemEntries(path);
        FileSystemEntries.Clear();
        foreach (var fileSystemEntry in entries)
        {
            FileSystemEntries.Add(fileSystemEntry);
        }

        OnPropertyChanged();
    }
}