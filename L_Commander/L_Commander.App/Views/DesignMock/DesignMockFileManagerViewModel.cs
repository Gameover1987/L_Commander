using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.OperatingSystem.Operations;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Factories;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.UI.Infrastructure;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.Views.DesignMock;

internal sealed class DesignMockWindowManager : IWindowManager
{
    public Task<string> ShowInputBox(string title, string message, MetroDialogSettings settings = null)
    {
        throw new NotImplementedException();
    }

    public Task<MessageDialogResult> ShowMessage(string title, string message)
    {
        throw new NotImplementedException();
    }

    public Task<MessageDialogResult> ShowQuestion(string title, string message)
    {
        throw new NotImplementedException();
    }

    public Task<ProgressDialogController> ShowProgressDialog(string title, string message)
    {
        throw new NotImplementedException();
    }
}

internal sealed class DesignMockCopyOperation : ICopyOperation
{
    public bool IsBusy { get; }

    public Task Execute(FileSystemEntryDescriptor[] entries, string destDirectory)
    {
        throw new NotImplementedException();
    }

    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public event EventHandler<CopyProgressEventArgs> Progress;
}

internal sealed class DesignMockFileManagerTabViewModel : FileManagerTabViewModel
{
    public DesignMockFileManagerTabViewModel()
        : base(new FolderFilterViewModel(), new FileSystemProvider(new IconCache()), new DesignMockWindowManager(), new OperatingSystemProvider(), new DesignMockExceptionHandler(), new FolderWatcher(), new UiTimer())
    {
        Initialize("C:\\");

        foreach (var fileSystemEntryViewModel in FileSystemEntries)
        {
            fileSystemEntryViewModel.Initialize();
        }

        SelectedFileSystemEntry = FileSystemEntries.FirstOrDefault();
    }
}

internal sealed class DesignMockExceptionHandler : IExceptionHandler
{
    public void HandleExceptionWithMessageBox(Exception exception)
    {
        throw new NotImplementedException();
    }

    public void HandleException(Exception exception)
    {
        throw new NotImplementedException();
    }
}

internal sealed class DesignMockViewModelFactory : ViewModelFactory
{
    public DesignMockViewModelFactory()
        : base(new FileSystemProvider(new IconCache()), new DesignMockWindowManager(), new OperatingSystemProvider(), new DesignMockExceptionHandler())
    {

    }
}

internal sealed class DesignMockFileManagerViewModel : FileManagerViewModel
{
    public DesignMockFileManagerViewModel()
        : base(new FileSystemProvider(new IconCache()), new ClipBoardProvider(), new DesignMockViewModelFactory(), new OperatingSystemProvider())
    {
        Initialize(new FileManagerSettings
        {
            Tabs = new TabSettings[]
            {
                new TabSettings 
                {
                    Path = @"C:\", 
                    IsLocked = true 
                },
                 new TabSettings
                {
                    Path = @"E:\Download",
                    IsLocked = false
                }
            },SelectedPath = @"C:\"
        });
    }
}