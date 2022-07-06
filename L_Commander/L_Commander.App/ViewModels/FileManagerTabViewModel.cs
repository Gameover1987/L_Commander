using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.Infrastructure;
using L_Commander.UI.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.ViewModels;

public class FileManagerTabViewModel : ViewModelBase, IFileManagerTabViewModel
{
    private const int TimerInterval = 1000;

    private readonly IFolderFilterViewModel _folderFolderFilter;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IWindowManager _windowManager;
    private readonly IOperatingSystemProvider _operatingSystemProvider;
    private readonly IExceptionHandler _exceptionHandler;
    private readonly IFolderWatcher _folderWatcher;
    private readonly IUiTimer _timer;
    private string _fullPath;

    private IFileSystemEntryViewModel _selectedFileSystemEntry;
    private IFileSystemEntryViewModel[] _selectedFileSystemEntries = Array.Empty<IFileSystemEntryViewModel>();
    private readonly List<NavigationHistoryItem> _navigationHistory = new List<NavigationHistoryItem>();
    private int _navigationIndex;

    private readonly ListCollectionView _folderView;
    private readonly object _lock = new object();

    private bool _isBusy;
    private bool _isLocked;

    public FileManagerTabViewModel(IFolderFilterViewModel folderFolderFilter,
        IFileSystemProvider fileSystemProvider,
        IWindowManager windowManager,
        IOperatingSystemProvider operatingSystemProvider,
        IExceptionHandler exceptionHandler,
        IFolderWatcher folderWatcher,
        IUiTimer timer)
    {
        _folderFolderFilter = folderFolderFilter;
        _folderFolderFilter.Changed += FolderFolderFilterOnChanged;
        _fileSystemProvider = fileSystemProvider;
        _windowManager = windowManager;
        _operatingSystemProvider = operatingSystemProvider;
        _exceptionHandler = exceptionHandler;
        _folderWatcher = folderWatcher;
        _timer = timer;
        _timer.Initialize(TimeSpan.FromMilliseconds(TimerInterval));
        _timer.Tick += TimerOnTick;
        _folderWatcher.Changed += FolderWatcherOnChanged;

        RenameCommand = new DelegateCommand(RenameCommandHandler, CanRenameCommandHandler);
        OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
        MakeDirCommand = new DelegateCommand(MakeDirCommandHandler, CanMakeDirCommandHandler);
        DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);

        RefreshCommand = new DelegateCommand(RefreshCommandHandler, CanRefreshCommandHandler);
        BackCommand = new DelegateCommand(BackCommandHandler, CanBackCommandHandler);
        NextCommand = new DelegateCommand(NextCommandHandler, CanNextCommandHandler);
        TopCommand = new DelegateCommand(TopCommandHandler, CanTopCommandHandler);

        BindingOperations.EnableCollectionSynchronization(FileSystemEntries, _lock);

        _folderView = (ListCollectionView)CollectionViewSource.GetDefaultView(FileSystemEntries);
        _folderView.Filter = FileSystemEntryFilter;
        _folderView.SortDescriptions.Add(new SortDescription(nameof(IFileSystemEntryViewModel.FileOrFolder), ListSortDirection.Ascending));
        _folderView.SortDescriptions.Add(new SortDescription(nameof(IFileSystemEntryViewModel.Name), ListSortDirection.Ascending));
    }

    public string FullPath => _fullPath;

    public string ShortPath => _fileSystemProvider.GetDirectoryName(_fullPath);

    public string Name => _fileSystemProvider.GetDirectoryName(_fullPath);

    public bool IsLocked
    {
        get { return _isLocked; }
        set
        {
            if (_isLocked == value)
                return;
            _isLocked = value;
            OnPropertyChanged(() => IsLocked);
        }
    }

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

    public ObservableCollection<IFileSystemEntryViewModel> FileSystemEntries { get; } = new ObservableCollection<IFileSystemEntryViewModel>();

    public ListCollectionView FolderView => _folderView;

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

    public IFileSystemEntryViewModel[] SelectedEntries
    {
        get { return _selectedFileSystemEntries; }
        set
        {
            _selectedFileSystemEntries = value;
            OnPropertyChanged(() => SelectedEntries);
        }
    }

    public IFolderFilterViewModel FolderFilter => _folderFolderFilter;

    public IDelegateCommand RenameCommand { get; }

    public IDelegateCommand OpenCommand { get; }

    public IDelegateCommand MakeDirCommand { get; }

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

    private async void SetPath(string rootPath)
    {
        if (rootPath != null)
            _folderWatcher.EndWatch(_fullPath);

        _fullPath = rootPath;
        _folderWatcher.BeginWatch(_fullPath);

        IsBusy = true;
        FileSystemEntries.Clear();
        _folderFolderFilter.Clear();
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

            _folderFolderFilter.Refresh(FileSystemEntries);

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

    private bool CanRenameCommandHandler()
    {
        if (SelectedFileSystemEntry == null)
            return false;

        return !IsBusy;
    }

    private async void RenameCommandHandler()
    {
        try
        {
            var settings = new MetroDialogSettings { DefaultText = SelectedFileSystemEntry.Name };
            var newName = await _windowManager.ShowInputBox("Rename", SelectedFileSystemEntry.FullPath, settings);
            if (newName.IsNullOrWhiteSpace())
                return;

            _fileSystemProvider.Rename(SelectedFileSystemEntry.FileOrFolder, SelectedFileSystemEntry.FullPath, newName);
        }
        catch (Exception exception)
        {
            _exceptionHandler.HandleExceptionWithMessageBox(exception);
        }
    }

    private bool CanOpenCommandHandler()
    {
        return SelectedFileSystemEntry != null;
    }

    private void OpenCommandHandler()
    {
        if (SelectedFileSystemEntry.IsFile)
        {
            _operatingSystemProvider.OpenFile(SelectedFileSystemEntry.FullPath);
        }
        else
        {
            var path = SelectedFileSystemEntry.FullPath;
            SetPath(path);
            _navigationHistory.Add(NavigationHistoryItem.Create(path));
            _navigationIndex++;
        }
    }

    private bool CanMakeDirCommandHandler()
    {
        return !IsBusy;
    }

    private async void MakeDirCommandHandler()
    {
        try
        {
            var newName = await _windowManager.ShowInputBox($"Make directory in \r\n'{FullPath}'", "Name");
            if (newName.IsNullOrWhiteSpace())
                return;

            var newDirectoryFullPath = _fileSystemProvider.CombinePaths(FullPath, newName);
            _fileSystemProvider.CreateDirectory(newDirectoryFullPath);

        }
        catch (Exception exception)
        {
            _exceptionHandler.HandleExceptionWithMessageBox(exception);
        }
    }

    private bool CanDeleteCommandHandler()
    {
        return SelectedFileSystemEntry != null;
    }

    private async void DeleteCommandHandler()
    {
        try
        {
            var titleMsg = SelectedFileSystemEntry.IsFile ? "Delete file?" : "Delete folder?";
            if (SelectedEntries.Length > 1)
                titleMsg = $"Delete selected items ({SelectedEntries.Length})?";

            var dialogResult = await _windowManager.ShowQuestion(titleMsg, string.Join(Environment.NewLine, SelectedEntries.Select(x => x.FullPath).OrderBy(x => x)));
            if (dialogResult != MessageDialogResult.Affirmative)
                return;

            IsBusy = true;
            foreach (var entry in SelectedEntries)
            {
                _fileSystemProvider.Delete(entry.FileOrFolder, entry.FullPath);
            }
            IsBusy = false;
        }
        catch (Exception exception)
        {
            _exceptionHandler.HandleExceptionWithMessageBox(exception);
        }
    }    

    private bool CanRefreshCommandHandler()
    {
        return !IsBusy;
    }

    private void RefreshCommandHandler()
    {
        SetPath(FullPath);
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

    private void FolderFolderFilterOnChanged(object sender, EventArgs e)
    {
        _folderView.Refresh();
    }

    private void FolderWatcherOnChanged(object sender, FileChangedEventArgs e)
    {
        ExecuteInUIThread(() =>
        {
            IsBusy = true;
            _timer.Stop();
            try
            {
                switch (e.Change)
                {
                    case FileChnageType.Create:
                        var newEntry = CreateFileSystemEntryViewModel(e.CurrentPath);
                        newEntry.Initialize();
                        FileSystemEntries.Add(newEntry);
                        break;

                    case FileChnageType.Delete:
                        var deletedEntry = FileSystemEntries.FirstOrDefault(x => x.FullPath == e.CurrentPath);
                        if (deletedEntry != null)
                        {
                            FileSystemEntries.Remove(deletedEntry);
                        }
                        break;

                    case FileChnageType.Rename:
                        var renamedEntry = FileSystemEntries.FirstOrDefault(x => x.FullPath == e.OldPath);
                        renamedEntry?.Rename(e.CurrentPath);
                        break;
                }
                _timer.Start();
            }
            catch (Exception exception)
            {
                _exceptionHandler.HandleException(exception);
            }
        });
    }

    private void TimerOnTick(object sender, EventArgs e)
    {
        _timer.Stop();
        IsBusy = false;
        FolderFilter.Refresh(FileSystemEntries);
        FolderView.Refresh();
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

        if (!_folderFolderFilter.IsCorrespondsByFilter(entry))
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

        public override string ToString()
        {
            return string.Format($"[{Path}] at [{DateTime}]");
        }
    }
}