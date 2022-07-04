using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels.Filtering;

public class FolderFilterViewModel : ViewModelBase, IFolderFilterViewModel
{
    public FolderFilterViewModel()
    {
        var filtersView = (ListCollectionView)CollectionViewSource.GetDefaultView(Filters);
        filtersView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FilterItemViewModel.Group)));
        filtersView.SortDescriptions.Add(new SortDescription(nameof(FilterItemViewModel.Order), ListSortDirection.Ascending));
        filtersView.SortDescriptions.Add(new SortDescription(nameof(FilterItemViewModel.Group), ListSortDirection.Ascending));

        ResetFiltersCommand = new DelegateCommand(ResetFiltersCommandHandler, CanResetFiltersCommandHandler);
        CheckOrUncheckGroupCommand = new DelegateCommand(CheckOrUncheckGroupCommandHandler);
    }

    public ObservableCollection<FilterItemViewModel> Filters { get; } = new ObservableCollection<FilterItemViewModel>();
    public bool HasFilters => Filters.Any();

    public IDelegateCommand ResetFiltersCommand { get; }

    public IDelegateCommand CheckOrUncheckGroupCommand { get; }

    public event EventHandler<EventArgs> Changed;

    public void Clear()
    {
        foreach (var filterItemViewModel in Filters)
        {
            filterItemViewModel.Checked -= FilterItemViewModelOnChecked;
        }
        Filters.Clear();

        OnPropertyChanged(() => HasFilters);
    }

    public void Refresh(IEnumerable<IFileSystemEntryViewModel> fileSystemEntries)
    {
        var groups = fileSystemEntries
            .Where(x => x.IsFile)
            .GroupBy(x => x.Extension.ToLower())
            .OrderBy(x => x.Key)
            .ToArray();

        foreach (var filterItemViewModel in Filters)
        {
            filterItemViewModel.Checked -= FilterItemViewModelOnChecked;
        }
        Filters.Clear();
        foreach (var grouping in groups)
        {
            var filterItemViewModel = new FilterItemViewModel();
            var extension = grouping.Key;
            filterItemViewModel.Extension = extension;
            if (FolderFilterGroups.Items.ContainsKey(extension))
            {
                filterItemViewModel.Group = FolderFilterGroups.Items[extension];
            }
            else
            {
                filterItemViewModel.Group = "Other";
                filterItemViewModel.Order = 1;
            }

            filterItemViewModel.Checked += FilterItemViewModelOnChecked;

            Filters.Add(filterItemViewModel);
        }

        OnPropertyChanged(() => HasFilters);
    }

    public bool IsCorrespondsByFilter(IFileSystemEntryViewModel fileSystemEntry)
    {
        if (!fileSystemEntry.IsFile)
            return true;

        if (!Filters.Any())
            return true;

        return Filters.Where(x => x.IsChecked).Select(x => x.Extension).Contains(fileSystemEntry.Extension.ToLower());

    }

    private bool CanResetFiltersCommandHandler()
    {
        return !Filters.All(x => x.IsChecked);
    }

    private void ResetFiltersCommandHandler()
    {
        foreach (var filterItemViewModel in Filters)
        {
            filterItemViewModel.SetIsChecked(true);
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void CheckOrUncheckGroupCommandHandler(object obj)
    {
        var groupName = obj.ToString();
        var itemsByGroup = Filters.Where(x => x.Group == groupName).ToArray();
        if (itemsByGroup.All(x => x.IsChecked))
        {
            foreach (var filterItemViewModel in itemsByGroup)
            {
                filterItemViewModel.IsChecked = false;
            }
        }
        else
        {
            foreach (var filterItemViewModel in itemsByGroup)
            {
                filterItemViewModel.IsChecked = true;
            }
        }
    }

    private void FilterItemViewModelOnChecked(object sender, EventArgs e)
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}