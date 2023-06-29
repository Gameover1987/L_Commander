using L_Commander.App.OperatingSystem;
using L_Commander.UI.ViewModels;
using System;

namespace L_Commander.App.ViewModels
{
    public class FavoriteNavigatingEventArgs : EventArgs
    {
        public FavoriteNavigatingEventArgs(string navigationPath)
        {
            NavigationPath = navigationPath;
        }

        public string NavigationPath { get; }
    }

    public interface IFavoriteFileSystemEntryViewModel 
    {
        string DisplayName { get; }

        string FullPath { get; }

        event EventHandler<FavoriteNavigatingEventArgs> Navigating;

        void Navigate();
    }

    public class FavoriteFileSystemEntryViewModel : ViewModelBase, IFavoriteFileSystemEntryViewModel
    {
        private readonly FileSystemEntryDescriptor _descriptor;

        public FavoriteFileSystemEntryViewModel(FileSystemEntryDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public string DisplayName => _descriptor.Name;

        public string FullPath => _descriptor.Path;

        public event EventHandler<FavoriteNavigatingEventArgs> Navigating;

        public void Navigate()
        {
            Navigating?.Invoke(this, new FavoriteNavigatingEventArgs(_descriptor.Path));
        }
    }

}
