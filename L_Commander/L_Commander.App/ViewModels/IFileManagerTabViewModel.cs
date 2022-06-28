using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels;

public interface IFileManagerTabViewModel
{
    string CurrentPath { get; }

    void Initialize(string rootPath);

    IDelegateCommand OpenCommand { get; }
}