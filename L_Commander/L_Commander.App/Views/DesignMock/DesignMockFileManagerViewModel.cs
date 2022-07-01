using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Factories;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.Views.DesignMock;

internal sealed class DesignMockWindowManager : IWindowManager
{
    public Task<string> ShowInputBox(string title, string message, MetroDialogSettings settings = null)
    {
        throw new NotImplementedException();
    }
}

internal sealed class DesignMockFileManagerTabViewModel : FileManagerTabViewModel
{
    public DesignMockFileManagerTabViewModel()
        : base(new FileSystemProvider(new IconCache()), new DesignMockWindowManager())
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
        : base(new FileSystemProvider(new IconCache()), new ClipBoardProvider(), new OperatingSystemProvider(), new DesignMockWindowManager(), new DesignMockIconCache())
    {
        Initialize(new FileManagerSettings { Paths = new[] { "C:\\", "D:\\" } });
    }
}