using System;
using System.IO;
using System.Linq;
using L_Commander.App.FileSystem;
using L_Commander.App.Infrastructure;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock;

internal sealed class DesignMockDriveInfoProvider : IDriveInfoProvider
{
    public DriveInfo[] GetDrives()
    {
        return DriveInfo.GetDrives();
    }
}

internal sealed class DesignMockFileManagerTabViewModel : FileManagerTabViewModel
{
    public DesignMockFileManagerTabViewModel() 
        : base(new FileSystemProvider())
    {
        Initialize("C:\\");

        foreach (var fileSystemEntryViewModel in FileSystemEntries)
        {
            fileSystemEntryViewModel.Initialize();
        }

        SelectedFileSystemEntry = FileSystemEntries.FirstOrDefault();
    }
}

internal sealed class DesignMockFileManagerViewModel : FileManagerViewModel
{
    public DesignMockFileManagerViewModel()
        : base(new DriveInfoProvider())
    {
        Initialize(new FileManagerSettings{Paths = new []{"C:\\", "D:\\"}});
    }
}