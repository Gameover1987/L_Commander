using L_Commander.UI.Commands;
using System.Windows.Data;

namespace L_Commander.App.ViewModels;

public interface IFileManagerTabViewModel
{
    string FullPath { get; }

    string ShortPath { get; }

    ListCollectionView FolderView { get; }

    IDelegateCommand RenameCommand { get; }
    IDelegateCommand OpenCommand { get; }
    IDelegateCommand CopyCommand { get; }
    IDelegateCommand MakeDirCommand { get; }
    IDelegateCommand DeleteCommand { get; }

    void Initialize(string rootPath);
}