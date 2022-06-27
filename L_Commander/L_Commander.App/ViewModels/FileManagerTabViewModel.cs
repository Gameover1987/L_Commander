using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using L_Commander.App.FileSystem;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileManagerTabViewModel : ViewModelBase, IFileManagerTabViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private string _rootPath;
    private IFileSystemEntryViewModel _selectedFileSystemEntry;

    public FileManagerTabViewModel(IFileSystemProvider fileSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;
    }

    public string RootPath => _rootPath;

    public ObservableCollection<IFileSystemEntryViewModel> FileSystemEntries { get; } = new ObservableCollection<IFileSystemEntryViewModel>();

    public IFileSystemEntryViewModel SelectedFileSystemEntry
    {
        get { return _selectedFileSystemEntry; }
        set
        {
            if (_selectedFileSystemEntry == value)
                return;
            _selectedFileSystemEntry = value;
            OnPropertyChanged();
        }
    }

    public void SetPath(string rootPath)
    {
        _rootPath = rootPath;

        var entries = _fileSystemProvider.GetFileSystemEntries(rootPath);
        FileSystemEntries.Clear();
        foreach (var path in entries)
        {
            FileSystemEntries.Add(CreateFileSystemEntryViewModel(path));
        }

        OnPropertyChanged();
    }

    protected virtual IFileSystemEntryViewModel CreateFileSystemEntryViewModel(string path)
    {
       return new FileSystemEntryViewModel(path, _fileSystemProvider);
    }
}