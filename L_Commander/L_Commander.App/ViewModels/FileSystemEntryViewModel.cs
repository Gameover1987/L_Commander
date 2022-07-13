using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileSystemEntryViewModel : ViewModelBase, IFileSystemEntryViewModel
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IExceptionHandler _exceptionHandler;
    private readonly IContextMenuItemProvider _contextMenuItemProvider;

    private string _fullPath;
    private FileSystemEntryDescriptor _descriptor;
    private bool _isBusy;
    private long _totalSize;

    public FileSystemEntryViewModel(string fullPath, IFileSystemProvider fileSystemProvider, IExceptionHandler exceptionHandler, IContextMenuItemProvider contextMenuItemProvider)
    {
        _fullPath = fullPath;
        _fileSystemProvider = fileSystemProvider;
        _exceptionHandler = exceptionHandler;
        _contextMenuItemProvider = contextMenuItemProvider;
    }

    public ImageSource Icon { get; private set; }

    public string FullPath => _fullPath;

    public string Name { get; private set; }

    public bool IsBusy
    {
        get { return _isBusy; }
        set
        {
            if (_isBusy == value)
                return;
            _isBusy = value;
            OnPropertyChanged(() => IsBusy);
        }
    }

    public bool IsFile => FileOrFolder == FileOrFolder.File;

    public bool IsSystem { get; private set; }

    public bool IsHidden { get; private set; }

    public bool IsInitialized { get; private set; }

    public FileOrFolder FileOrFolder { get; private set; }

    public string Extension { get; private set; }

    public long TotalSize
    {
        get { return _totalSize; }
        private set
        {
            if (_totalSize == value)
                return;
            _totalSize = value;
            OnPropertyChanged(() => TotalSize);
        }
    }

    public DateTime Created { get; private set; }

    public ObservableCollection<ContextMenuItemViewModel> ContextMenuItems { get; } = new ObservableCollection<ContextMenuItemViewModel>();

    public void Initialize()
    {
        InitializeImpl();
    }

    public async void CalculateFolderSize()
    {
        if (IsFile)
            return;

        try
        {
            IsBusy = true;
            TotalSize = await Task.Run(() =>
            {
                Thread.Sleep(1000);
                return _fileSystemProvider.CalculateFolderSize(FullPath);
            });
        }
        catch (Exception exception)
        {
            _exceptionHandler.HandleExceptionWithMessageBox(exception);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public FileSystemEntryDescriptor GetDescriptor()
    {
        return _descriptor;
    }

    protected virtual void InitializeImpl()
    {
        _descriptor = _fileSystemProvider.GetFileSystemDescriptor(_fullPath);

        FileOrFolder = _descriptor.FileOrFolder;
        IsHidden = _descriptor.IsHidden;
        IsSystem = _descriptor.IsSystem;
        Name = _descriptor.Name;
        Created = _descriptor.Created;

        ContextMenuItems.Clear();
        foreach (var menuItemViewModel in _contextMenuItemProvider.GetMenuItems(_descriptor))
        {
            ContextMenuItems.Add(menuItemViewModel);
        }

        if (_descriptor.IsFile)
        {
            Extension = _descriptor.Extension;
            TotalSize = _descriptor.TotalSize;
            Icon = _descriptor.Icon;
        }
        else
        {
            TotalSize = -1;
        }

        IsInitialized = true;

        OnPropertyChanged();
    }

    public void Rename(string newPath)
    {
        _fullPath = newPath;

        InitializeImpl();
    }
}