using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels;

public interface IFileSystemEntryViewModel
{
    ImageSource Icon { get; }

    string FullPath { get; }

    string Name { get; }

    string Extension { get; }

    long TotalSize { get; }

    DateTime Created { get; }

    bool IsInitialized { get; }

    FileOrFolder FileOrFolder { get; }

    bool IsFile { get; }

    bool IsSystem { get; }

    bool IsHidden { get; }

    void Initialize();

    void CalculateFolderSize();

    void Rename(string newPath);

    FileSystemEntryDescriptor GetDescriptor();

    ObservableCollection<ContextMenuItemViewModel> ContextMenuItems { get; }
}