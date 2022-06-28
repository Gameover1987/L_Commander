using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using L_Commander.App.FileSystem;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileManagerTabViewModel : ViewModelBase, IFileManagerTabViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private string _currentPath;
    private IFileSystemEntryViewModel _selectedFileSystemEntry;
    private readonly List<NavigationHistoryItem> _navigationHistory = new List<NavigationHistoryItem>();
    private int _navigationIndex;

    private ListCollectionView _fileSystemView;
    private IEnumerable<IFileSystemEntryViewModel> _fileSystemEntries;

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

    private bool FileSystemEntryFilter(object obj)
    {
        var entry = (IFileSystemEntryViewModel)obj;
        entry.Initialize();
        if (entry.IsHidden)
            return false;

        if (entry.IsSystem)
            return false;

        return true;
    }

    public string CurrentPath => _currentPath;

    public IEnumerable<IFileSystemEntryViewModel> FileSystemEntries
    {
        get { return _fileSystemEntries; }
        set
        {
            _fileSystemEntries = value;

            _fileSystemView = (ListCollectionView) CollectionViewSource.GetDefaultView(FileSystemEntries);
            _fileSystemView.Filter = FileSystemEntryFilter;

            OnPropertyChanged(() => FileSystemEntries);
        }
    }

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

    public void Initialize(string rootPath)
    {
        _navigationHistory.Clear();
        _navigationIndex = 0;
        _navigationHistory.Add(NavigationHistoryItem.Create(rootPath));

        SetPath(rootPath);
    }

    private void SetPath(string rootPath)
    {
        _currentPath = rootPath;

        var entries = _fileSystemProvider.GetFileSystemEntries(rootPath);
        FileSystemEntries = entries.Select(x => new FileSystemEntryViewModel(x, _fileSystemProvider));

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
            var path = SelectedFileSystemEntry.Path;
            SetPath(path);
            _navigationHistory.Add(NavigationHistoryItem.Create(path));
            _navigationIndex++;
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
        SetPath(CurrentPath);
    }

    private bool CanBackCommandHandler()
    {
        if (_navigationHistory.Count == 0)
            return false;

        return _navigationIndex > 0;
    }

    private void BackCommandHandler()
    {
        _navigationIndex--;
        SetPath(_navigationHistory[_navigationIndex].Path);
    }

    private bool CanNextCommandHandler()
    {
        if (_navigationHistory.Count == 0)
            return false;

        return _navigationIndex >= 0 &&
               _navigationIndex < _navigationHistory.Count - 1;
    }

    private void NextCommandHandler()
    {
        _navigationIndex++;
        SetPath(_navigationHistory[_navigationIndex].Path);
    }

    private bool CanTopCommandHandler()
    {
        return !_fileSystemProvider.IsDriveRoot(CurrentPath);
    }

    private void TopCommandHandler()
    {
        var topLevelPath = _fileSystemProvider.GetTopLevelPath(CurrentPath);
        SetPath(topLevelPath);
        _navigationHistory.Add(NavigationHistoryItem.Create(topLevelPath));
        _navigationIndex++;
    }

    private class NavigationHistoryItem
    {
        public string Path { get; private set; }

        public DateTime DateTime { get; private set; }

        public static NavigationHistoryItem Create(string path)
        {
            return new NavigationHistoryItem
            {
                Path = path,
                DateTime = DateTime.Now
            };
        }

        public override bool Equals(object obj)
        {
            var other = (NavigationHistoryItem)obj;

            return Path == other.Path;
        }

        public override string ToString()
        {
            return string.Format($"[{Path}] at [{DateTime}]");
        }
    }
}