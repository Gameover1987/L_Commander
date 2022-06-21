using System.Collections.ObjectModel;
using System.Linq;
using L_Commander.App.FileSystem;

namespace L_Commander.App.ViewModels;

public class FileManagerViewModel : IFileManagerViewModel
{
    private readonly IDriveInfoProvider _driveInfoProvider;

    public FileManagerViewModel(IDriveInfoProvider driveInfoProvider)
    {
        _driveInfoProvider = driveInfoProvider;
    }

    public ObservableCollection<DriveViewModel> Drives { get; } = new ObservableCollection<DriveViewModel>();

    public ObservableCollection<IFileManagerTabViewModel> Tabs { get; } = new ObservableCollection<IFileManagerTabViewModel>();

    public void Initialize()
    {
        Drives.Clear();

        var driveInfos = _driveInfoProvider.GetDrives();
        foreach (var driveInfo in driveInfos)
        {
            Drives.Add(new DriveViewModel(driveInfo));
        }

        Tabs.Clear();
        var fileManagerTabViewModel = new FileManagerTabViewModel(new FileSystemProvider());
        fileManagerTabViewModel.SetPath(Drives.First().RootPath);
        Tabs.Add(fileManagerTabViewModel);
    }
}