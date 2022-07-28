using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using L_Commander.App.Infrastructure;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels.Filtering;

public class FolderFilterViewModel : ViewModelBase, IFolderFilterViewModel
{
    private const string TagsGroupName = "Tags";
    private const string WithoutTagsGroupName = "Without tags";

    private readonly ITagRepository _tagRepository;

    private FilterItemViewModel _withoutTagFilterItemViewModel;

    public FolderFilterViewModel(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
        var filtersView = (ListCollectionView)CollectionViewSource.GetDefaultView(Filters);
        filtersView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FilterItemViewModel.Group)));
        filtersView.SortDescriptions.Add(new SortDescription(nameof(FilterItemViewModel.Order), ListSortDirection.Ascending));
        filtersView.SortDescriptions.Add(new SortDescription(nameof(FilterItemViewModel.Group), ListSortDirection.Ascending));

        SelectAllFiltersCommand = new DelegateCommand(SelectAllFiltersCommandHandler, CanSelectAllFiltersCommandHandler);
        ClearAllFiltersCommand = new DelegateCommand(ClearAllFiltersCommandHandler, CanClearAllFiltersCommandHandler);
        CheckOrUncheckGroupCommand = new DelegateCommand(CheckOrUncheckGroupCommandHandler);
    }

    public ObservableCollection<FilterItemViewModel> Filters { get; } = new ObservableCollection<FilterItemViewModel>();

    public bool HasFilters => Filters.Any();

    public IDelegateCommand SelectAllFiltersCommand { get; }

    public IDelegateCommand ClearAllFiltersCommand { get; }

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
            .Where(x => x != null)
            .Where(x => x.IsFile)
            .GroupBy(x => x.Extension.ToLower())
            .OrderBy(x => x.Key)
            .ToArray();

        foreach (var filterItemViewModel in Filters)
        {
            filterItemViewModel.Checked -= FilterItemViewModelOnChecked;
        }

        Filters.Clear();

        _withoutTagFilterItemViewModel = CreateWithoutTagFilterItemViewModel();

        Filters.Add(_withoutTagFilterItemViewModel);
        foreach (var tag in _tagRepository.GetAllTags())
        {
            var filterItemViewModel = CreateTagFilterItemViewModel(tag);
            Filters.Add(filterItemViewModel);
        }

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

    private FilterItemViewModel CreateTagFilterItemViewModel(Tag tag)
    {
        var filterItemViewModel = new FilterItemViewModel();
        filterItemViewModel.Extension = tag.Text;
        filterItemViewModel.Tag = tag;
        filterItemViewModel.Group = TagsGroupName;
        filterItemViewModel.Order = -1;
        filterItemViewModel.Checked += FilterItemViewModelOnChecked;
        return filterItemViewModel;
    }

    private FilterItemViewModel CreateWithoutTagFilterItemViewModel()
    {
        var withoutTagFilterItemViewModel = new FilterItemViewModel();
        withoutTagFilterItemViewModel.Extension = WithoutTagsGroupName;
        withoutTagFilterItemViewModel.Tag = null;
        withoutTagFilterItemViewModel.Group = TagsGroupName;
        withoutTagFilterItemViewModel.Order = -1;
        withoutTagFilterItemViewModel.Checked += FilterItemViewModelOnChecked;
        return withoutTagFilterItemViewModel;
    }

    public bool IsCorrespondsByFilter(IFileSystemEntryViewModel fileSystemEntry)
    {
        if (!Filters.Any())
            return true;

        if (Filters.All(x => x.IsChecked == false))
            return false;

        var checkedTagIds = Filters.Where(x => x.Group == TagsGroupName && x.IsChecked && x.Tag != null).Select(x => x.Tag.Guid).ToArray();
        if (checkedTagIds.Length > 0 && fileSystemEntry.Tags.Count > 0)
        {
            if (!fileSystemEntry.Tags.Select(x => x.Guid).IntersectsWith(checkedTagIds))
            {
                return false;
            }
        }

        if (checkedTagIds.Length == 0 && fileSystemEntry.Tags.Count > 0)
        {
            return false;
        }

        if (!_withoutTagFilterItemViewModel.IsChecked && fileSystemEntry.Tags.Count == 0)
            return false;

        if (!fileSystemEntry.IsFile)
            return true;

        return Filters.Where(x => x.IsChecked).Select(x => x.Extension).Contains(fileSystemEntry.Extension.ToLower());

    }

    private bool CanClearAllFiltersCommandHandler()
    {
        return Filters.Any(x => x.IsChecked);
    }

    private void ClearAllFiltersCommandHandler()
    {
        foreach (var filterItemViewModel in Filters)
        {
            filterItemViewModel.SetIsChecked(false);
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    private bool CanSelectAllFiltersCommandHandler()
    {
        return !Filters.All(x => x.IsChecked);
    }

    private void SelectAllFiltersCommandHandler()
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
                filterItemViewModel.SetIsChecked(false);
            }
        }
        else
        {
            foreach (var filterItemViewModel in itemsByGroup)
            {
                filterItemViewModel.SetIsChecked(true);
            }
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void FilterItemViewModelOnChecked(object sender, EventArgs e)
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}