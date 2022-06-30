using L_Commander.App.Infrastructure;

namespace L_Commander.App.ViewModels
{
    public interface IFileManagerViewModel
    {
        IFileManagerTabViewModel SelectedTab { get; }

        void Initialize(FileManagerSettings settings);

        FileManagerSettings CollectSettings();
    }
}