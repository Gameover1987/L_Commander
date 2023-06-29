using System;
using L_Commander.App.Infrastructure.Settings;

namespace L_Commander.App.ViewModels
{
    public interface IFileManagerViewModel : IDisposable
    {
        IFileManagerTabViewModel SelectedTab { get; }

        void Initialize(FileManagerSettings settings);

        void SwapTabs(IFileManagerTabViewModel sourceTab, IFileManagerTabViewModel targetTab);

        FileManagerSettings CollectSettings();
    }
}