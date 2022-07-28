using System;
using System.Linq;
using L_Commander.App.Infrastructure;
using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels;

public class ContextMenuItemProvider : IContextMenuItemProvider
{
    private readonly ITagRepository _tagRepository;
    private readonly IMainViewModel _mainViewModel;

    public ContextMenuItemProvider(ITagRepository tagRepository, IMainViewModel mainViewModel)
    {
        _tagRepository = tagRepository;
        _mainViewModel = mainViewModel;
    }

    public ContextMenuItemViewModel[] GetMenuItems(IFileSystemEntryViewModel fileSystemEntry)
    {
        var allTags = _tagRepository.GetAllTags();

        var tagsByDescriptor = _tagRepository.GetTagsByPath(fileSystemEntry.GetDescriptor());

        var tagsSubMenu = new ContextMenuItemViewModel
        {
            Header = "Tags"
        };
        foreach (var tag in allTags)
        {
            tagsSubMenu.Children.Add(new ContextMenuItemViewModel
            {
                Header = tag,
                IsCheckable = true,
                IsChecked = tagsByDescriptor.Contains(tag, TagEqualityComparer.Instance),
                Command = new DelegateCommand(obj =>
                {
                    var item = (ContextMenuItemViewModel)obj;
                    item.IsChecked = !item.IsChecked;
                    _tagRepository.SetTagsForPath(fileSystemEntry.FullPath, tagsSubMenu.Children
                        .Where(x => x.IsChecked)
                        .Select(x => (Tag)x.Header).ToArray());
                    fileSystemEntry.UpdateTags();
                })
            });
        }

        tagsSubMenu.Children.Add(new SeparatorContextMenuItemViewModel());
        tagsSubMenu.Children.Add(new ContextMenuItemViewModel
        {
            Header = "Remove all tags",
            IsCheckable = false,
            IsChecked = false,
            Command = new DelegateCommand(obj =>
            {
                _tagRepository.SetTagsForPath(fileSystemEntry.FullPath, Array.Empty<Tag>());
                fileSystemEntry.UpdateTags();
            }, obj =>
            {
                return tagsByDescriptor.Any();
            })
        });

        if (fileSystemEntry.IsFile)
        {
            return new ContextMenuItemViewModel[]
            {
                new ContextMenuItemViewModel { Header = "Open", IsDefault = true, GestureText = "Enter", Command = _mainViewModel.ActiveFileManager.SelectedTab.OpenCommand},
                new ContextMenuItemViewModel { Header = "Open with", Command = _mainViewModel.ActiveFileManager.SelectedTab.OpenWithCommand},
                new SeparatorContextMenuItemViewModel(),
                tagsSubMenu,
                new SeparatorContextMenuItemViewModel(),
                new ContextMenuItemViewModel { Header = "Paste", GestureText = "Ctrl+V" },
                new ContextMenuItemViewModel { Header = "Copy", GestureText = "Ctrl+C" },
                new ContextMenuItemViewModel { Header = "Move", GestureText = "Ctrl+X" },
                new SeparatorContextMenuItemViewModel(),
                new ContextMenuItemViewModel { Header = "Delete", GestureText = "Del" },
                new SeparatorContextMenuItemViewModel(),
                new ContextMenuItemViewModel { Header = "Properties", GestureText = "Alt+Enter", Command = _mainViewModel.ActiveFileManager.SelectedTab.ShowPropertiesCommand},
            };
        }

        return new ContextMenuItemViewModel[]
        {
            new ContextMenuItemViewModel { Header = "Open", IsDefault = true, GestureText = "Enter", Command = _mainViewModel.ActiveFileManager.SelectedTab.OpenCommand },
            new SeparatorContextMenuItemViewModel(),
            tagsSubMenu,
            new SeparatorContextMenuItemViewModel(),
            new ContextMenuItemViewModel { Header = "Paste", GestureText = "Ctrl+V" },
            new ContextMenuItemViewModel { Header = "Copy", GestureText = "Ctrl+C"},
            new ContextMenuItemViewModel { Header = "Move", GestureText = "Ctrl+X"},
            new SeparatorContextMenuItemViewModel(),
            new ContextMenuItemViewModel { Header = "Delete", GestureText = "Del"},
            new SeparatorContextMenuItemViewModel(),
            new ContextMenuItemViewModel { Header = "Properties", GestureText = "Alt+Enter", Command = _mainViewModel.ActiveFileManager.SelectedTab.ShowPropertiesCommand},
        };
    }
}