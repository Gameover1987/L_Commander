using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels;

public interface IFileManagerTabViewModel
{
    string FullPath { get; }

    string ShortPath { get; }

    void Initialize(string rootPath);

    IDelegateCommand OpenCommand { get; }
}