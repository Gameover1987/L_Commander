using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels.Filtering
{
    public interface IFolderFilterViewModel
    {
        void Clear();

        void Initialize(IEnumerable<IFileSystemEntryViewModel> fileSystemEntries);

        ObservableCollection<FilterItemViewModel> Filters { get; }

        bool IsInitialized { get; }

        IDelegateCommand ResetFiltersCommand { get; }

        event EventHandler<EventArgs> Changed;

        bool IsCorrespondsByFilter(IFileSystemEntryViewModel fileSystemEntry);
    }
}
