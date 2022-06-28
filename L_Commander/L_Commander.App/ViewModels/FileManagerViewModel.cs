using System.Collections.ObjectModel;
using System.Linq;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileManagerViewModel : ViewModelBase, IFileManagerViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IClipBoardProvider _clipBoardHelper;
    private readonly IOperatingSystemProvider _operatingSystemProvider;

    private IFileManagerTabViewModel _selectedTab;

    public FileManagerViewModel(IFileSystemProvider fileSystemProvider, IClipBoardProvider clipBoardHelper, IOperatingSystemProvider operatingSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;
        _clipBoardHelper = clipBoardHelper;
        _operatingSystemProvider = operatingSystemProvider;
        ChangeDriveCommand = new DelegateCommand(x => ChangeDriveCommandHandler(x), x => CanChangeDriveCommandHandler());
        NewTabCommand = new DelegateCommand(NewTabCommandHandler, CanNewTabCommandHandler);
        CloseTabCommand = new DelegateCommand(x => CloseTabCommandHandler(x), x => CanCloseTabCommandHandler());
        CopyPathCommand = new DelegateCommand(CopyPathCommandHandler);
        OpenInExplorerCommand = new DelegateCommand(OpenInExplorerCommandHandler);
        OpenInTerminalCommand = new DelegateCommand(OpenInTerminalCommandHandler);
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

        Tabs.Clear();
        if (settings != null && settings.Paths.Any())
        {
            foreach (var path in settings.Paths)
            {
                Tabs.Add(CreateFileManagerTabViewModel(path));
            }
        }
        else
        {
            var fileManagerTabViewModel = CreateFileManagerTabViewModel(Drives.First().RootPath);
            Tabs.Add(fileManagerTabViewModel);
        }

        SelectedTab = Tabs.First();
    }

    public FileManagerSettings CollectSettings()
    {
        return new FileManagerSettings
        {
            Paths = Tabs.Select(x => x.CurrentPath).ToArray()
        };
    }

    protected virtual IFileManagerTabViewModel CreateFileManagerTabViewModel(string path)
    {
        var fileManagerTabViewModel = new FileManagerTabViewModel(_fileSystemProvider);
        fileManagerTabViewModel.Initialize(path);

        return fileManagerTabViewModel;
    }

    private bool CanChangeDriveCommandHandler()
    {
        return true;
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
        var newTab = CreateFileManagerTabViewModel(SelectedTab.CurrentPath);
        Tabs.Add(newTab);
    }

    private bool CanCloseTabCommandHandler()
    {
        return Tabs.Count > 1;
    }

    private void CloseTabCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;

        Tabs.Remove(tab);
    }

    private void CopyPathCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;
        _clipBoardHelper.CopyToClipBoard(tab.CurrentPath);
    }

    private void OpenInExplorerCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;
        _operatingSystemProvider.OpenExplorer(tab.CurrentPath);
    }

    private void OpenInTerminalCommandHandler(object obj)
    {
        var tab = (IFileManagerTabViewModel)obj;
        _operatingSystemProvider.OpenTerminal(tab.CurrentPath);
    }
}