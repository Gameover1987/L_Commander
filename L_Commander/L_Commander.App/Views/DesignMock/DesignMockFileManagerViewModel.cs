﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Factories;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.Common.Extensions;
using L_Commander.UI.Infrastructure;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.Views.DesignMock;

internal sealed class DesignMockWindowManager : IWindowManager
{
    public MetroWindow MainWindow { get; }
    public MetroWindow ActiveWindow { get; }

    public Task<string> ShowInputBox(string title, string message, MetroDialogSettings settings = null)
    {
        throw new NotImplementedException();
    }

    public Task<MessageDialogResult> ShowMessage(string title, string message, MetroDialogSettings settings = null)
    {
        throw new NotImplementedException();
    }

    public Task<MessageDialogResult> ShowQuestion(string title, string message, MetroDialogSettings settings = null)
    {
        throw new NotImplementedException();
    }

    public Task<ProgressDialogController> ShowProgressDialog(string title, string message)
    {
        throw new NotImplementedException();
    }

    public Task ShowDialogAsync<T>(object dataContext) where T : BaseMetroDialog, new()
    {
        throw new NotImplementedException();
    }

    public bool ShowDialogWindow<T>(object dataContext) where T : Window
    {
        throw new NotImplementedException();
    }

    public void ShowWindow<T>(object dataContext) where T : Window
    {
        throw new NotImplementedException();
    }
}

internal sealed class DesignMockFileManagerTabViewModel : FileManagerTabViewModel
{
    public DesignMockFileManagerTabViewModel()
        : base(new FolderFilterViewModel(new DesignMockTagRepository()),
            new FileSystemProvider(new IconCache()), 
            new DesignMockWindowManager(), 
            new ProcessProvider(),
            new DesignMockExceptionHandler(), 
            new FolderWatcher(new FileSystemProvider(new IconCache())), 
            new UiTimer(),
            new FileSystemEntryViewModelFactory(new FileSystemProvider(new IconCache()), new DesignMockExceptionHandler(), new DesignMockTagRepository()),
            new DesignMockTagRepository(),
            new DesignMockOpenWithViewModel(),
            new TabStatusBarViewModel(),
            new DesignMockSettingsManager(),
            new DesignMockHistoryManager())
    {
        ThreadTaskExtensions.IsSyncRun = true;
        Initialize("E:\\Download");

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

internal sealed class DesignMockFileManagerTabViewModelFactory : FileManagerTabViewModelFactory
{
    public DesignMockFileManagerTabViewModelFactory()
        : base(new FileSystemProvider(new IconCache()),
            new DesignMockWindowManager(), 
            new ProcessProvider(), 
            new DesignMockExceptionHandler(),
            new FileSystemEntryViewModelFactory(new FileSystemProvider(new IconCache()), new DesignMockExceptionHandler(), new DesignMockTagRepository()), 
            new DesignMockTagRepository(),
            new DesignMockOpenWithViewModel(),
            new DesignMockSettingsManager(),
            new DesignMockHistoryManager())
    {

    }
}

internal sealed class DesignMockFileManagerViewModel : FileManagerViewModel
{
    public DesignMockFileManagerViewModel()
        : base(new FileSystemProvider(new IconCache()), new ClipBoardProvider(), new DesignMockFileManagerTabViewModelFactory(), new ProcessProvider())
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
            }, 
            SelectedPath = @"C:\"
        });
    }
}