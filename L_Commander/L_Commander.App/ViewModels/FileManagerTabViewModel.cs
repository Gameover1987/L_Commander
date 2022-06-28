using System.Collections.ObjectModel;
using System.Diagnostics;
using L_Commander.App.FileSystem;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileManagerTabViewModel : ViewModelBase, IFileManagerTabViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private string _currentPath;
    private IFileSystemEntryViewModel _selectedFileSystemEntry;

    public FileManagerTabViewModel(IFileSystemProvider fileSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;

        OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
        DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);
        
        RefreshCommand = new DelegateCommand(RefreshCommandHandler, CanRefreshCommandHandler);
        BackCommand = new DelegateCommand(BackCommandHandler, CanBackCommandHandler);
        NextCommand = new DelegateCommand(NextCommandHandler, CanNextCommandHandler);
        TopCommand = new DelegateCommand(TopCommandHandler, CanTopCommandHandler);
    }

    public string CurrentPath => _currentPath;

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

    public IDelegateCommand RefreshCommand { get; }

    public IDelegateCommand BackCommand { get; }

    public IDelegateCommand NextCommand { get; }

    public IDelegateCommand TopCommand { get; }

    public void SetPath(string rootPath)
    {
        _currentPath = rootPath;

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

    private bool CanRefreshCommandHandler()
    {
        return true;
    }

    private void RefreshCommandHandler()
    {

    }

    private bool CanBackCommandHandler()
    {
        return true;
    }

    private void BackCommandHandler()
    {

    }

    private bool CanNextCommandHandler()
    {
        return true;
    }

    private void NextCommandHandler()
    {

    }

    private bool CanTopCommandHandler()
    {
        return !_fileSystemProvider.IsDriveRoot(CurrentPath);
    }

    private void TopCommandHandler()
    {
        SetPath(_fileSystemProvider.GetTopLevelPath(CurrentPath));
    }
}