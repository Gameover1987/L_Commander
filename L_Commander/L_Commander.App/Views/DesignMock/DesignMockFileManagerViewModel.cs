using System;
using System.IO;
using System.Linq;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock;

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
        : base(new FileSystemProvider(), new ClipBoardProvider(), new OperatingSystemProvider())
    {
        Initialize(new FileManagerSettings{Paths = new []{"C:\\", "D:\\"}});
    }
}