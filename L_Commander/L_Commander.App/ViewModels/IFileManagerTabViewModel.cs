using System;
using L_Commander.UI.Commands;
using System.Windows.Data;
using L_Commander.App.ViewModels.Filtering;

namespace L_Commander.App.ViewModels;

public interface IFileManagerTabViewModel : IDisposable
{
    string FullPath { get; }

    string ShortPath { get; }

    bool IsLocked { get; set; }

    IFileSystemEntryViewModel[] SelectedEntries { get; set; }

    ListCollectionView FolderView { get; }

    IFolderFilterViewModel FolderFilter { get; }

    event EventHandler DeletedExternally;

    IDelegateCommand RenameCommand { get; }
    IDelegateCommand OpenCommand { get; }
    IDelegateCommand MakeDirCommand { get; }    

    void Initialize(string rootPath);

    void ReLoad();
}