using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.History;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels.Factories;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.App.Views;
using L_Commander.App.Views.Controls;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.Infrastructure;
using L_Commander.UI.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.ViewModels;

public class FileManagerTabViewModel : ViewModelBase, IFileManagerTabViewModel
{
    private const int FileSystemEntriesPageSize = 20;
    private const int TimerInterval = 1000;

    private readonly IFolderFilterViewModel _folderFilter;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IWindowManager _windowManager;
    private readonly IProcessProvider _processProvider;
    private readonly IExceptionHandler _exceptionHandler;
    private readonly IFolderWatcher _folderWatcher;
    private readonly IUiTimer _timer;
    private readonly IFileSystemEntryViewModelFactory _fileSystemEntryViewModelFactory;
    private readonly ITagRepository _tagRepository;
    private readonly IOpenWithViewModel _openWithViewModel;
    private readonly ITabStatusBarViewModel _statusBarViewModel;
    private readonly ISettingsManager _settingsManager;
    private readonly IHistoryManager _historyManager;
    private string _fullPath;

    private bool _isManualyChangedSelection;
    private IFileSystemEntryViewModel _selectedFileSystemEntry;
    private IFileSystemEntryViewModel[] _selectedFileSystemEntries = Array.Empty<IFileSystemEntryViewModel>();
    private readonly List<NavigationHistoryItem> _navigationHistory = new List<NavigationHistoryItem>();
    private int _navigationIndex;

    private readonly ListCollectionView _folderView;
    private readonly object _lock = new object();

    private bool _isBusy;
    private bool _isLocked;
    private FilesAndFoldersSettings _settings;

    public FileManagerTabViewModel(IFolderFilterViewModel folderFilter,
        IFileSystemProvider fileSystemProvider,
        IWindowManager windowManager,
        IProcessProvider processProvider,
        IExceptionHandler exceptionHandler,
        IFolderWatcher folderWatcher,
        IUiTimer timer,
        IFileSystemEntryViewModelFactory fileSystemEntryViewModelFactory,
        ITagRepository tagRepository,
        IOpenWithViewModel openWithViewModel,
        ITabStatusBarViewModel statusBarViewModel,
        ISettingsManager settingsManager,
        IHistoryManager historyManager)
    {
        _folderFilter = folderFilter;
        _folderFilter.Changed += FolderFilterOnChanged;
        _fileSystemProvider = fileSystemProvider;
        _windowManager = windowManager;
        _processProvider = processProvider;
        _exceptionHandler = exceptionHandler;
        _folderWatcher = folderWatcher;
        _folderWatcher.Changed += FolderWatcherOnChanged;
        _timer = timer;
        _fileSystemEntryViewModelFactory = fileSystemEntryViewModelFactory;
        _tagRepository = tagRepository;
        _openWithViewModel = openWithViewModel;
        _statusBarViewModel = statusBarViewModel;
        _settingsManager = settingsManager;
        _historyManager = historyManager;
        _settingsManager.SettingsChanged += SettingsManagerOnSettingsChanged;
        _timer.Initialize(TimeSpan.FromMilliseconds(TimerInterval));
        _timer.Tick += TimerOnTick;

        RenameCommand = new DelegateCommand(RenameCommandHandler, CanRenameCommandHandler);
        OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
        OpenWithCommand = new DelegateCommand(OpenWithCommandHandler, CanOpenWithCommandHandler);
        MakeDirCommand = new DelegateCommand(MakeDirCommandHandler, CanMakeDirCommandHandler);
        ShowPropertiesCommand = new DelegateCommand(ShowPropertiesCommandHandler);
        CalculateFolderSizeCommand = new DelegateCommand(CalculateFolderSizeCommandHandler, CanCalculateFolderSizeCommandHandler);

        RefreshCommand = new DelegateCommand(RefreshCommandHandler, CanRefreshCommandHandler);
        BackCommand = new DelegateCommand(BackCommandHandler, CanBackCommandHandler);
        NextCommand = new DelegateCommand(NextCommandHandler, CanNextCommandHandler);
        TopCommand = new DelegateCommand(TopCommandHandler, CanTopCommandHandler);
        NavigateCommand = new DelegateCommand(NavigateCommandHandler, CanNavigateCommandHandler);

        BindingOperations.EnableCollectionSynchronization(FileSystemEntries, _lock);

        _folderView = (ListCollectionView)CollectionViewSource.GetDefaultView(FileSystemEntries);
        _folderView.Filter = FileSystemEntryFilter;
        _folderView.SortDescriptions.Add(new SortDescription(nameof(IFileSystemEntryViewModel.FileOrFolder), ListSortDirection.Ascending));
        _folderView.SortDescriptions.Add(new SortDescription(nameof(IFileSystemEntryViewModel.Extension), ListSortDirection.Ascending));
        _folderView.SortDescriptions.Add(new SortDescription(nameof(IFileSystemEntryViewModel.Name), ListSortDirection.Ascending));
    }

    public string FullPath
    {
        get { return _fullPath; }
        set
        {
            if (_fullPath == value)
                return;

            if (_fileSystemProvider.IsDirectoryExists(value))
            {
                _fullPath = value;
                OnPropertyChanged(() => FullPath);

                SetPath(_fullPath);
                _navigationHistory.Add(NavigationHistoryItem.Create(FullPath));
                _navigationIndex++;
            }
        }
    }

    public ObservableCollection<FileSystemPathPartViewModel> PathByParts { get; } = new ObservableCollection<FileSystemPathPartViewModel>();

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
            _isManualyChangedSelection = true;
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

            _statusBarViewModel.Update(this);
        }
    }

    public ITabStatusBarViewModel StatusBar => _statusBarViewModel;

    public IFolderFilterViewModel FolderFilter => _folderFilter;

    public event EventHandler DeletedExternally;

    public IDelegateCommand RenameCommand { get; }

    public IDelegateCommand OpenCommand { get; }

    public IDelegateCommand OpenWithCommand { get; }

    public IDelegateCommand MakeDirCommand { get; }
    public IDelegateCommand ShowPropertiesCommand { get; }

    public IDelegateCommand CalculateFolderSizeCommand { get; }

    public IDelegateCommand RefreshCommand { get; }

    public IDelegateCommand BackCommand { get; }

    public IDelegateCommand NextCommand { get; }

    public IDelegateCommand TopCommand { get; }

    public IDelegateCommand NavigateCommand { get; }

    public void Initialize(string rootPath)
    {
        _navigationHistory.Clear();
        _navigationIndex = 0;
        _navigationHistory.Add(NavigationHistoryItem.Create(rootPath));

        _settings = _settingsManager.Get().FilesAndFoldersSettings;

        SetPath(rootPath);
    }

    public void Dispose()
    {
        _folderWatcher.EndWatch();
        _folderWatcher.Dispose();
    }

    public void ReLoad()
    {
        SetPath(FullPath);
    }

    private async void SetPath(string rootPath)
    {
        try
        {
            if (rootPath != null)
                _folderWatcher.EndWatch();

            _fullPath = rootPath;
            _folderWatcher.BeginWatch(_fullPath);

            FillPathByParts(_fullPath);

            IsBusy = true;
            FileSystemEntries.Clear();

            _folderFilter.Clear();

            var fileSystemEntries = new List<IFileSystemEntryViewModel>();
            await ThreadTaskExtensions.Run(() =>
            {
                var items = _fileSystemProvider
                    .GetFileSystemEntries(rootPath)
                    .Select(x => _fileSystemEntryViewModelFactory.CreateEntryViewModel(x, this));
                var filesWithTags = _tagRepository.GetAllFilesWithTags();
                foreach (var fileSystemEntryViewModel in items)
                {
                    var fileWithTags = filesWithTags.FirstOrDefault(x => x.FilePath == fileSystemEntryViewModel.FullPath);
                    fileSystemEntryViewModel.Initialize(fileWithTags?.Tags);
                    fileSystemEntries.Add(fileSystemEntryViewModel);

                    if (fileSystemEntries.Count > FileSystemEntriesPageSize)
                    {
                        foreach (var fileSystemEntry in fileSystemEntries)
                        {
                            FileSystemEntries.Add(fileSystemEntry);
                        }

                        fileSystemEntries.Clear();
                    }
                }

                if (fileSystemEntries.Any())
                {
                    foreach (var fileSystemEntry in fileSystemEntries)
                    {
                        FileSystemEntries.Add(fileSystemEntry);
                    }
                }

                if (!_isManualyChangedSelection)
                    SelectedFileSystemEntry = FileSystemEntries.FirstOrDefault(FileSystemEntryFilter);
            });

            _folderFilter.Refresh(FileSystemEntries);
        }
        finally
        {
            DelegateCommand.NotifyCanExecuteChangedForAll();
            IsBusy = false;
        }

        OnPropertyChanged();
    }

    private void FillPathByParts(string path)
    {
        var parts = _fileSystemProvider.GetPathByParts(path);
        PathByParts.Clear();
        foreach (var fileSystemEntryDescriptor in parts)
        {
            PathByParts.Add(new FileSystemPathPartViewModel(fileSystemEntryDescriptor, NavigateCommand));
        }
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
            _historyManager.Add("Rename operation", $"Old name is'{SelectedFileSystemEntry.FullPath}'\r\nNew name is '{newName}'");
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
            _processProvider.OpenFile(SelectedFileSystemEntry.FullPath);
        }
        else
        {
            var path = SelectedFileSystemEntry.FullPath;
            SetPath(path);
            _navigationHistory.Add(NavigationHistoryItem.Create(path));
            _navigationIndex++;
        }
    }

    private bool CanOpenWithCommandHandler()
    {
        if (SelectedFileSystemEntry == null)
            return false;

        return SelectedFileSystemEntry.IsFile;
    }

    private async void OpenWithCommandHandler()
    {
        var fullPath = SelectedFileSystemEntry.FullPath;
        _openWithViewModel.Initialize(fullPath);

        await _windowManager.ShowDialogAsync<OpenWithDialog>(_openWithViewModel);
        if (_openWithViewModel.IsCancelled)
            return;

        _processProvider.OpenFileWith(fullPath, _openWithViewModel.SelectedApp);
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
            _historyManager.Add("Make directory", $"New directory name '{newName}'");

        }
        catch (Exception exception)
        {
            _exceptionHandler.HandleExceptionWithMessageBox(exception);
        }
    }

    private void ShowPropertiesCommandHandler()
    {
        if (SelectedEntries.Length == 1)
        {
            _processProvider.ShowPropertiesByPath(SelectedFileSystemEntry.FullPath);
        }
        else
        {
            var multiplePropertiesViewModel = _fileSystemEntryViewModelFactory.CreateMultiplePropertiesViewModel();
            multiplePropertiesViewModel.Initialize(SelectedEntries);
            _windowManager.ShowWindow<MultipleFilePropertiesWindow>(multiplePropertiesViewModel);
        }
    }

    private bool CanCalculateFolderSizeCommandHandler()
    {
        if (IsBusy)
            return false;

        return SelectedEntries.Any(x => x.FileOrFolder == FileOrFolder.Folder);
    }

    private void CalculateFolderSizeCommandHandler()
    {
        foreach (var selectedEntry in SelectedEntries)
        {
            selectedEntry.CalculateFolderSize();
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

    private bool CanNavigateCommandHandler(object obj)
    {
        return true;
    }

    private void NavigateCommandHandler(object obj)
    {
        var pathPartViewModel = (FileSystemPathPartViewModel)obj;
        SetPath(pathPartViewModel.Path);
        _navigationHistory.Add(NavigationHistoryItem.Create(pathPartViewModel.Path));
        _navigationIndex++;
    }

    private void FolderFilterOnChanged(object sender, EventArgs e)
    {
        _folderView.Refresh();
    }

    private void FolderWatcherOnChanged(object sender, FileChangedEventArgs e)
    {
        ExecuteInUIThread(() =>
        {
            try
            {
                if (e.Change == FileChangeType.Delete && e.CurrentPath == FullPath)
                {
                    IsBusy = false;
                    _timer.Stop();
                    FileSystemEntries.Clear();
                    DeletedExternally?.Invoke(this, EventArgs.Empty);
                }

                IsBusy = true;
                _timer.Stop();

                switch (e.Change)
                {
                    case FileChangeType.Create:
                        var newEntry = _fileSystemEntryViewModelFactory.CreateEntryViewModel(e.CurrentPath, this);
                        newEntry.Initialize();
                        FileSystemEntries.Add(newEntry);
                        break;

                    case FileChangeType.Delete:
                        var deletedEntry = FileSystemEntries.FirstOrDefault(x => x.FullPath == e.CurrentPath);
                        if (deletedEntry != null)
                        {
                            FileSystemEntries.Remove(deletedEntry);
                        }
                        break;

                    case FileChangeType.Rename:
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

        if (entry.IsHidden && !_settings.ShowHiddenFilesAndFolders)
            return false;

        if (entry.IsSystem && !_settings.ShowSystemFilesAndFolders)
            return false;

        if (!_folderFilter.IsCorrespondsByFilter(entry))
        {
            return false;
        }

        return true;
    }

    private void SettingsManagerOnSettingsChanged(object sender, SettingsChangedEventArgs e)
    {
        _settings = e.Settings.FilesAndFoldersSettings;
        if (IsBusy)
            return;

        _folderView.Refresh();
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