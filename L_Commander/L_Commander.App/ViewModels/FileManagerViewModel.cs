using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels.Factories;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileManagerViewModel : ViewModelBase, IFileManagerViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IClipBoardProvider _clipBoardHelper;
    private readonly IFileManagerTabViewModelFactory _fileManagerTabViewModelFactory;
    private readonly IProcessProvider _processProvider;

    private IFileManagerTabViewModel _selectedTab;

    public FileManagerViewModel(IFileSystemProvider fileSystemProvider, IClipBoardProvider clipBoardHelper, IFileManagerTabViewModelFactory fileManagerTabViewModelFactory, IProcessProvider processProvider)
    {
        _fileSystemProvider = fileSystemProvider;
        _fileSystemProvider.DrivesChanged += FileSystemProviderOnDrivesChanged;
        _clipBoardHelper = clipBoardHelper;
        _fileManagerTabViewModelFactory = fileManagerTabViewModelFactory;
        _processProvider = processProvider;

        ChangeDriveCommand = new DelegateCommand(ChangeDriveCommandHandler, CanChangeDriveCommandHandler);
        NewTabCommand = new DelegateCommand(NewTabCommandHandler, CanNewTabCommandHandler);
        CloseTabCommand = new DelegateCommand(CloseTabCommandHandler, CanCloseTabCommandHandler);
        CloseAllButThisTabCommand = new DelegateCommand(CloseAllButThisTabCommandHandler, CanCloseAllButThisTabCommandHandler);
        LockTabCommand = new DelegateCommand(LockTabCommandHandler);
        CopyPathCommand = new DelegateCommand(CopyPathCommandHandler);
        OpenInExplorerCommand = new DelegateCommand(OpenInExplorerCommandHandler);
        OpenInTerminalCommand = new DelegateCommand(OpenInTerminalCommandHandler);

        Tabs.CollectionChanged += TabsOnCollectionChanged;
    }

    private void TabsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        
    }

    public ObservableCollection<DriveViewModel> Drives { get; } = new ObservableCollection<DriveViewModel>();

    public ObservableCollection<IFileManagerTabViewModel> Tabs { get; } = new ObservableCollection<IFileManagerTabViewModel>();

    public IFileManagerTabViewModel SelectedTab
    {
        get { return _selectedTab; }
        set
        {
            if (_selectedTab == value)
                return;
            _selectedTab = value;
            OnPropertyChanged(() => SelectedTab);
        }
    }
    public IDelegateCommand ChangeDriveCommand { get; }
    public IDelegateCommand NewTabCommand { get; }
    public IDelegateCommand CloseTabCommand { get; }
    public IDelegateCommand CloseAllButThisTabCommand { get; }
    public IDelegateCommand LockTabCommand { get; }
    public IDelegateCommand CopyPathCommand { get; }
    public IDelegateCommand OpenInExplorerCommand { get; }
    public IDelegateCommand OpenInTerminalCommand { get; }

    public void Initialize(FileManagerSettings settings)
    {
        Drives.Clear();

        var driveInfos = _fileSystemProvider.GetDrives();
        foreach (var driveInfo in driveInfos)
        {
            Drives.Add(new DriveViewModel(driveInfo));
        }

        Tabs.ForEach(x => x.DeletedExternally -= TabOnDeletedExternally);
        Tabs.Clear();
        if (settings != null && settings.Tabs?.Any() == true)
        {
            foreach (var tabSettings in settings.Tabs)
            {
                if (!_fileSystemProvider.IsDirectoryExists(tabSettings.Path))
                    continue;
                var tab = CreateFileManagerTabViewModel(tabSettings.Path);
                tab.IsLocked = tabSettings.IsLocked;
                Tabs.Add(tab);
            }

            var oldSelectedTab = Tabs.FirstOrDefault(x => x.FullPath == settings.SelectedPath);
            if (oldSelectedTab != null)
            {
                SelectedTab = oldSelectedTab;
            }
            else
            {
                SelectedTab = Tabs.First();
            }
        }
        else
        {
            var tab = CreateFileManagerTabViewModel(Drives.First().RootPath);
            Tabs.Add(tab);

            SelectedTab = Tabs.First();
        }
    }

    public FileManagerSettings CollectSettings()
    {
        return new FileManagerSettings
        {
            Tabs = Tabs.Select(x => new TabSettings { Path = x.FullPath, IsLocked = x.IsLocked }).ToArray(),
            SelectedPath = SelectedTab?.FullPath
        };
    }

    public void SwapTabs(IFileManagerTabViewModel sourceTab, IFileManagerTabViewModel targetTab)
    {
        var sourceIndex = Tabs.IndexOf(sourceTab);
        if (sourceIndex < 0)
            return;

        var targetIndex = Tabs.IndexOf(targetTab);
        if (targetIndex < 0)
            return;

        Tabs[sourceIndex] = targetTab;
        Tabs[targetIndex] = sourceTab;

        SelectedTab = sourceTab;
    }

    public void Dispose()
    {
        foreach (var tab in Tabs)
        {
            tab.Dispose();
        }
    }

    protected virtual IFileManagerTabViewModel CreateFileManagerTabViewModel(string path)
    {
        var tab = _fileManagerTabViewModelFactory.CreateFileManagerTab();
        tab.DeletedExternally += TabOnDeletedExternally;
        tab.Initialize(path);

        return tab;
    }

    private bool CanChangeDriveCommandHandler(object obj)
    {
        var drive = (DriveViewModel)obj;
        return drive.IsReady;
    }

    private void ChangeDriveCommandHandler(object obj)
    {
        var drive = (DriveViewModel)obj;
        SelectedTab.Initialize(drive.RootPath);
    }

    private bool CanNewTabCommandHandler()
    {
        return true;
    }

    private void NewTabCommandHandler()
    {
        var newTab = CreateFileManagerTabViewModel(SelectedTab.FullPath);
        var selectedIndex = Tabs.IndexOf(SelectedTab);

        Tabs.Insert(selectedIndex + 1, newTab);

        SelectedTab = newTab;
    }

    private bool CanCloseTabCommandHandler(object obj)
    {
        var clickedTab = (IFileManagerTabViewModel)obj;
        if (clickedTab == null)
            return false;

        if (clickedTab.IsLocked)
            return false;

        return Tabs.Count > 1;
    }

    private void CloseTabCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;
        tab.Dispose();

        Tabs.Remove(tab);
    }

    private bool CanCloseAllButThisTabCommandHandler(object obj)
    {
        var clickedTab = (IFileManagerTabViewModel)obj;
        if (Tabs.Count == 1)
            return false;

        return Tabs.Count(x => !x.IsLocked && x != clickedTab) > 0;
    }

    private void CloseAllButThisTabCommandHandler(object obj)
    {
        var clickedTab = (IFileManagerTabViewModel)obj;

        var tabsToRemove = Tabs.Where(x => !x.IsLocked && x != clickedTab).ToArray();

        foreach (var tab in tabsToRemove)
        {
            tab.Dispose();
            Tabs.Remove(tab);
        }
    }

    private void LockTabCommandHandler(object obj)
    {
        var clickedTab = (IFileManagerTabViewModel)obj;
        clickedTab.IsLocked.Invert();
    }

    private void CopyPathCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;
        _clipBoardHelper.CopyToClipBoard(tab.FullPath);
    }

    private void OpenInExplorerCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;

        _processProvider.OpenExplorer(tab.FullPath);
    }

    private void OpenInTerminalCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;
        _processProvider.OpenTerminal(tab.FullPath);
    }

    private void FileSystemProviderOnDrivesChanged(object sender, EventArgs e)
    {
        Drives.Clear();

        var driveInfos = _fileSystemProvider.GetDrives();
        foreach (var driveInfo in driveInfos)
        {
            Drives.Add(new DriveViewModel(driveInfo));
        }
    }

    private void TabOnDeletedExternally(object sender, EventArgs e)
    {
        var tab = (IFileManagerTabViewModel)sender;
        tab.Dispose();
        Tabs.Remove(tab);
    }
}