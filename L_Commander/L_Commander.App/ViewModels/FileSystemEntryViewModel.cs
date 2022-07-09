﻿using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using L_Commander.App.OperatingSystem;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileSystemEntryViewModel : ViewModelBase, IFileSystemEntryViewModel
{
    private string _fullPath;
    private readonly IFileSystemProvider _fileSystemProvider;
    private FileSystemEntryDescriptor _descriptor;

    public FileSystemEntryViewModel(string fullPath, IFileSystemProvider fileSystemProvider)
    {
        _fullPath = fullPath;
        _fileSystemProvider = fileSystemProvider;
    }

    public ImageSource Icon { get; private set; }

    public string FullPath => _fullPath;

    public string Name { get; private set; }

    public bool IsFile => FileOrFolder == FileOrFolder.File;

    public bool IsSystem { get; private set; }

    public bool IsHidden { get; private set; }

    public bool IsInitialized { get; private set; }

    public FileOrFolder FileOrFolder { get; private set; }

    public string Extension { get; private set; }

    public long TotalSize { get; private set; }

    public DateTime Created { get; private set; }

    public void Initialize()
    {
        InitializeImpl();
    }

    public FileSystemEntryDescriptor GetDescriptor()
    {
        return _descriptor;
    }

    protected virtual void InitializeImpl()
    {
        _descriptor = _fileSystemProvider.GetEntryDetails(_fullPath);

        FileOrFolder = _descriptor.FileOrFolder;

        if (_descriptor.IsFile)
        {
            Extension = _descriptor.Extension;

            var numberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = " " };
            TotalSize = _descriptor.TotalSize;

            Icon = _descriptor.Icon;
        }

        IsHidden = _descriptor.IsHidden;
        IsSystem = _descriptor.IsSystem;
        Name = _descriptor.Name;
        Created = _descriptor.Created;

        IsInitialized = true;

        OnPropertyChanged();
    }

    public void Rename(string newPath)
    {
        _fullPath = newPath;

        InitializeImpl();
    }
}