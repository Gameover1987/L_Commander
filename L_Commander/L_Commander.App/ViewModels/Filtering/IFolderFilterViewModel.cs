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
        ObservableCollection<FilterItemViewModel> Filters { get; }

        bool HasFilters { get; }

        IDelegateCommand ResetFiltersCommand { get; }

        event EventHandler<EventArgs> Changed;

        void Clear();

        void Refresh(IEnumerable<IFileSystemEntryViewModel> fileSystemEntries);

        bool IsCorrespondsByFilter(IFileSystemEntryViewModel fileSystemEntry);
    }
}
