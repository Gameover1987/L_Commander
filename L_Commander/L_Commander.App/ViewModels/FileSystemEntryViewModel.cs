using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
    private readonly ITagRepository _tagRepository;
    
    private FileSystemEntryDescriptor _descriptor;
    private bool _isBusy;
    private long _totalSize;

    public FileSystemEntryViewModel(FileSystemEntryDescriptor descriptor,
        IFileSystemProvider fileSystemProvider,
        IExceptionHandler exceptionHandler,
        ITagRepository tagRepository)
    {
        _descriptor = descriptor;
        _fileSystemProvider = fileSystemProvider;
        _exceptionHandler = exceptionHandler;
        _tagRepository = tagRepository;
    }

    public ImageSource Icon { get; private set; }

    public string FullPath => _descriptor.Path;

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

    public ObservableCollection<Tag> Tags { get; } = new ObservableCollection<Tag>();

    public string TagsDescription
    {
        get
        {
            if (Tags == null)
                return null;

            return string.Join(", ", Tags.Select(x => x.Text));
        }
    }

    public void Initialize(Tag[] tags = null)
    {
        InitializeImpl(tags);
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
                return _fileSystemProvider.GetPathInfoRecursively(FullPath).TotalSize;
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

    public void UpdateTags()
    {
        Tags.Clear();
        var tags = _tagRepository.GetTagsByPath(_descriptor);
        if (tags.Length == 0)
            return;

        foreach (var tag in tags)
        {
            Tags.Add(tag);
        }
    }

    public FileSystemEntryDescriptor GetDescriptor()
    {
        return _descriptor;
    }

    public void Rename(string newPath)
    {
        _descriptor = _fileSystemProvider.GetFileSystemDescriptor(newPath);

        InitializeImpl();
    }

    protected virtual void InitializeImpl(Tag[] tags = null)
    {
        FileOrFolder = _descriptor.FileOrFolder;
        IsHidden = _descriptor.IsHidden;
        IsSystem = _descriptor.IsSystem;
        Name = _descriptor.Name;
        Created = _descriptor.Created;

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

        Tags.Clear();
        if (tags != null)
        {
            foreach (var tag in tags)
            {
                Tags.Add(tag);
            }
        }

        IsInitialized = true;
    }
}