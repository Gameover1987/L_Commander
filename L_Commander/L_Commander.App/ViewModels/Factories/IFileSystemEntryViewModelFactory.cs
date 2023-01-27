using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels.Factories
{
    public interface IFileSystemEntryViewModelFactory
    {
        IFileSystemEntryViewModel CreateEntryViewModel(FileSystemEntryDescriptor descriptor);

        IFavoriteFileSystemEntryViewModel CreateFavorite(string path);

        IMultipleFilePropertiesViewModel CreateMultiplePropertiesViewModel();
    }
}
