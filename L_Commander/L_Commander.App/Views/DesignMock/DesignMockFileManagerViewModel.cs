using System;
using System.IO;
using L_Commander.App.FileSystem;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock;

internal sealed class DesignMockDriveInfoProvider : IDriveInfoProvider
{
    public DriveInfo[] GetDrives()
    {
        return DriveInfo.GetDrives();
    }
}

internal sealed class DesignMockFileManagerViewModel : FileManagerViewModel
{
    public DesignMockFileManagerViewModel()
        : base(new DriveInfoProvider())
    {
        Initialize();
    }
}