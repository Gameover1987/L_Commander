using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels.Filtering;

public class FolderFilterViewModel : IFolderFilterViewModel
{
    public FolderFilterViewModel()
    {
        ResetFiltersCommand = new DelegateCommand(ResetFiltersCommandHandler, CanResetFiltersCommandHandler);
    }

    public ObservableCollection<FilterItemViewModel> Filters { get; } = new ObservableCollection<FilterItemViewModel>();

    public bool IsInitialized { get; private set; }

    public IDelegateCommand ResetFiltersCommand { get; }

    public event EventHandler<EventArgs> Changed;

    public void Clear()
    {
        foreach (var filterItemViewModel in Filters)
        {
            filterItemViewModel.Checked -= FilterItemViewModelOnChecked;
        }
        Filters.Clear();

        IsInitialized = false;
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
            var filterItemViewModel = new FilterItemViewModel(grouping.Key);
            filterItemViewModel.Checked += FilterItemViewModelOnChecked;
            Filters.Add(filterItemViewModel);
        }

        IsInitialized = true;
    }

    public bool IsCorrespondsByFilter(IFileSystemEntryViewModel fileSystemEntry)
    {
        if (!fileSystemEntry.IsFile)
            return true;

        if (!Filters.Any())
            return true;

        return Filters.Where(x => x.IsChecked).Select(x => x.Name).Contains(fileSystemEntry.Extension.ToLower());

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

    private void FilterItemViewModelOnChecked(object sender, EventArgs e)
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}