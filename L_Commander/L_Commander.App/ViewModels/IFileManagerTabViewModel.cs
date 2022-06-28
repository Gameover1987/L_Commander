using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels;

public interface IFileManagerTabViewModel
{
    string CurrentPath { get; }

    void SetPath(string rootPath);

    IDelegateCommand OpenCommand { get; }
}