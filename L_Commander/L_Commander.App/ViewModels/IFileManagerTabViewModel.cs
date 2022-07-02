using L_Commander.UI.Commands;
using System.Windows.Data;

namespace L_Commander.App.ViewModels;

public interface IFileManagerTabViewModel
{
    string FullPath { get; }

    string ShortPath { get; }

    ListCollectionView FolderView { get; }

    IDelegateCommand OpenCommand { get; }
    IDelegateCommand DeleteCommand { get; }
    IDelegateCommand RenameCommand { get; }

    void Initialize(string rootPath);
}