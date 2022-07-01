using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.ViewModels;

public class FileManagerTabViewModel : ViewModelBase, IFileManagerTabViewModel
{
    private readonly IFolderFilterViewModel _folderFilter;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IWindowManager _windowManager;
    private readonly IOperatingSystemProvider _operatingSystemProvider;
    private string _fullPath;

    private readonly ObservableCollection<IFileSystemEntryViewModel> _fileSystemEntries = new ObservableCollection<IFileSystemEntryViewModel>();
    private IFileSystemEntryViewModel _selectedFileSystemEntry;
    private readonly List<NavigationHistoryItem> _navigationHistory = new List<NavigationHistoryItem>();
    private int _navigationIndex;

    private readonly ListCollectionView _fileSystemView;
    private readonly object _lock = new object();

    private bool _isBusy;

    public FileManagerTabViewModel(IFolderFilterViewModel folderFilter, IFileSystemProvider fileSystemProvider, IWindowManager windowManager, IOperatingSystemProvider operatingSystemProvider)
    {
        _folderFilter = folderFilter;
        _folderFilter.Changed += FolderFilterOnChanged;
        _fileSystemProvider = fileSystemProvider;
        _windowManager = windowManager;
        _operatingSystemProvider = operatingSystemProvider;

        OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
        DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);

        RefreshCommand = new DelegateCommand(RefreshCommandHandler, CanRefreshCommandHandler);
        RenameCommand = new DelegateCommand(RenameCommandHandler, CanRenameCommandHandler);
        BackCommand = new DelegateCommand(BackCommandHandler, CanBackCommandHandler);
        NextCommand = new DelegateCommand(NextCommandHandler, CanNextCommandHandler);
        TopCommand = new DelegateCommand(TopCommandHandler, CanTopCommandHandler);

        BindingOperations.EnableCollectionSynchronization(FileSystemEntries, _lock);

        _fileSystemView = (ListCollectionView)CollectionViewSource.GetDefaultView(FileSystemEntries);
        _fileSystemView.Filter = FileSystemEntryFilter;
        _fileSystemView.SortDescriptions.Add(new SortDescription(nameof(IFileSystemEntryViewModel.FileOrFolder), ListSortDirection.Ascending));
        _fileSystemView.SortDescriptions.Add(new SortDescription(nameof(IFileSystemEntryViewModel.Name), ListSortDirection.Ascending));
    }

    public string FullPath => _fullPath;

    public string ShortPath => _fileSystemProvider.GetDirectoryName(_fullPath);

    public string Name => _fileSystemProvider.GetDirectoryName(_fullPath);

    public bool IsBusy
    {
        get { return _isBusy; }
        private set
        {
            if (_isBusy == value)
                return;
            _isBusy = value;
            OnPropertyChanged(() => IsBusy);
        }
    }

    public ObservableCollection<IFileSystemEntryViewModel> FileSystemEntries { get { return _fileSystemEntries; } }

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

    public IFolderFilterViewModel Filter => _folderFilter;

    public IDelegateCommand OpenCommand { get; }

    public IDelegateCommand DeleteCommand { get; }

    public IDelegateCommand RenameCommand { get; }

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

    private async void SetPath(string rootPath)
    {
        _fullPath = rootPath;

        IsBusy = true;
        FileSystemEntries.Clear();
        _folderFilter.Clear();
        try
        {
            await ThreadTaskExtensions.Run(() =>
            {
                var items = _fileSystemProvider
                    .GetFileSystemEntries(rootPath)
                    .Select(CreateFileSystemEntryViewModel);
                foreach (var fileSystemEntryViewModel in items)
                {
                    fileSystemEntryViewModel.Initialize();
                    FileSystemEntries.Add(fileSystemEntryViewModel);
                }
            });

            _folderFilter.Initialize(FileSystemEntries);

            SelectedFileSystemEntry = FileSystemEntries.FirstOrDefault(FileSystemEntryFilter);
        }
        finally
        {
            DelegateCommand.NotifyCanExecuteChangedForAll();
            IsBusy = false;
        }

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
            _operatingSystemProvider.OpenFile(SelectedFileSystemEntry.Path);
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
        return !IsBusy;
    }

    private void RefreshCommandHandler()
    {
        SetPath(FullPath);
    }

    private bool CanRenameCommandHandler()
    {
        if (SelectedFileSystemEntry == null)
            return false;

        return !IsBusy;
    }

    private async void RenameCommandHandler()
    {
        var settings = new MetroDialogSettings { DefaultText = SelectedFileSystemEntry.Name };
        var newName = await _windowManager.ShowInputBox("Rename", SelectedFileSystemEntry.Path, settings);
        if (newName.IsNullOrWhiteSpace())
            return;

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
        return !_fileSystemProvider.IsDriveRoot(FullPath);
    }

    private void TopCommandHandler()
    {
        var topLevelPath = _fileSystemProvider.GetTopLevelPath(FullPath);
        SetPath(topLevelPath);
        _navigationHistory.Add(NavigationHistoryItem.Create(topLevelPath));
        _navigationIndex++;
    }

    private void FolderFilterOnChanged(object sender, EventArgs e)
    {
        _fileSystemView.Refresh();
    }

    private bool FileSystemEntryFilter(object obj)
    {
        var entry = (IFileSystemEntryViewModel)obj;
        if (entry == null)
            return false;

        if (entry.IsHidden)
            return false;

        if (entry.IsSystem)
            return false;

        if (!_folderFilter.IsCorrespondsByFilter(entry))
        {
            return false;
        }

        return true;
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