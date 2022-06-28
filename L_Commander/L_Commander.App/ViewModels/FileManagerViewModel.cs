using System.Collections.ObjectModel;
using System.Linq;
using L_Commander.App.FileSystem;
using L_Commander.App.Infrastructure;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileManagerViewModel : ViewModelBase, IFileManagerViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;

    private IFileManagerTabViewModel _selectedTab;

    public FileManagerViewModel(IFileSystemProvider fileSystemProvider)
    {
        _fileSystemProvider = fileSystemProvider;
        NewTabCommand = new DelegateCommand(NewTabCommandHandler, CanNewTabCommandHandler);
        ChangeDriveCommand = new DelegateCommand(x => ChangeDriveCommandHandler(x), x => CanChangeDriveCommandHandler());
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

    public IDelegateCommand NewTabCommand { get; }

    public IDelegateCommand ChangeDriveCommand { get; }

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

    private bool CanNewTabCommandHandler()
    {
        return true;
    }

    private void NewTabCommandHandler()
    {
        var newTab = CreateFileManagerTabViewModel(SelectedTab.CurrentPath);
        Tabs.Add(newTab);
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
}