using System;
using System.Windows.Media;
using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels;

public interface IFileSystemEntryViewModel
{
    ImageSource Icon { get; }

    string Path { get; }

    string Name { get; }

    string Extension { get; }

    string TotalSize { get; }

    DateTime Created { get; }

    bool IsInitialized { get; }

    FileOrFolder FileOrFolder { get; }

    bool IsFile { get; }

    bool IsSystem { get; }

    bool IsHidden { get; }

    void Initialize();
}