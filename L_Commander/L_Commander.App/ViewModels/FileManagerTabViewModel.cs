using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using L_Commander.App.FileSystem;
using L_Commander.UI.Commands;
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

        OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
        DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);
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

    public IDelegateCommand OpenCommand { get; }

    public IDelegateCommand DeleteCommand { get; }

    public void SetPath(string rootPath)
    {
        _rootPath = rootPath;

        var entries = _fileSystemProvider.GetFileSystemEntries(rootPath);
        FileSystemEntries.Clear();
        foreach (var path in entries)
        {
            FileSystemEntries.Add(CreateFileSystemEntryViewModel(path));
        }

        Debug.WriteLine("Items count = {0}", FileSystemEntries.Count);

        OnPropertyChanged();
    }

    protected virtual IFileSystemEntryViewModel CreateFileSystemEntryViewModel(string path)
    {
       return new FileSystemEntryViewModel(path, _fileSystemProvider);
    }

    private bool CanOpenCommandHandler()
    {
        return SelectedFileSystemEntry != null;
    }

    private void OpenCommandHandler()
    {
        if (SelectedFileSystemEntry.IsFile)
        {

        }
        else
        {
            SetPath(SelectedFileSystemEntry.Path);
        }
    }

    private bool CanDeleteCommandHandler()
    {
        return SelectedFileSystemEntry != null;
    }

    private void DeleteCommandHandler()
    {

    }
}