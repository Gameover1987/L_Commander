using System.Linq;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels;

public interface IContextMenuItemProvider
{
    public ContextMenuItemViewModel[] GetMenuItems(FileSystemEntryDescriptor descriptor);
}

public class ContextMenuItemProvider : IContextMenuItemProvider
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly ITagRepository _tagRepository;

    public ContextMenuItemProvider(ISettingsProvider settingsProvider, ITagRepository tagRepository)
    {
        _settingsProvider = settingsProvider;
        _tagRepository = tagRepository;
    }

    public ContextMenuItemViewModel[] GetMenuItems(FileSystemEntryDescriptor descriptor)
    {
        var tagSettings = _settingsProvider.Get().TagSettings;

        var tagsByDescriptor = _tagRepository.GetTags(descriptor);

        var tagsSubMenu = new ContextMenuItemViewModel
        {
            DisplayName = "Tags",
            IsEnabled = tagSettings.IsEnabled,
        };
        foreach (var tag in tagSettings.Tags)
        {
            tagsSubMenu.Children.Add(new ContextMenuItemViewModel
            {
                Data = tag,
                IsCheckable = true,
                IsChecked = tagsByDescriptor.Contains(tag, TagEqualityComparer.Instance),
                Command = new DelegateCommand(obj =>
                {
                    var item = (ContextMenuItemViewModel)obj;
                    item.IsChecked = !item.IsChecked;
                    _tagRepository.SetTags(descriptor.Path, tagsSubMenu.Children
                        .Where(x => x.IsChecked)
                        .Select(x => (Tag)x.Data).ToArray());
                })
            });
        }

        if (descriptor.IsFile)
        {
            return new ContextMenuItemViewModel[]
            {
                new ContextMenuItemViewModel { DisplayName = "Open", IsDefault = true, GestureText = "Enter" },
                new ContextMenuItemViewModel { DisplayName = "Open with" },
                new SeparatorContextMenuItemViewModel(),
                tagsSubMenu,
                new SeparatorContextMenuItemViewModel(),
                new ContextMenuItemViewModel { DisplayName = "Copy", GestureText = "Ctrl+C" },
                new ContextMenuItemViewModel { DisplayName = "Move", GestureText = "Ctrl+V" },
                new SeparatorContextMenuItemViewModel(),
                new ContextMenuItemViewModel { DisplayName = "Delete", GestureText = "Del" },
                new SeparatorContextMenuItemViewModel(),
                new ContextMenuItemViewModel { DisplayName = "Properties", GestureText = "Alt+Enter" },
            };
        }

        return new ContextMenuItemViewModel[]
        {
            new ContextMenuItemViewModel { DisplayName = "Open", IsDefault = true, GestureText = "Enter"},
            new SeparatorContextMenuItemViewModel(),
            tagsSubMenu,
            new SeparatorContextMenuItemViewModel(),
            new ContextMenuItemViewModel { DisplayName = "Copy", GestureText = "Ctrl+C"},
            new ContextMenuItemViewModel { DisplayName = "Move", GestureText = "Ctrl+V"},
            new SeparatorContextMenuItemViewModel(),
            new ContextMenuItemViewModel { DisplayName = "Delete", GestureText = "Del"},
            new SeparatorContextMenuItemViewModel(),
            new ContextMenuItemViewModel { DisplayName = "Properties", GestureText = "Alt+Enter"},
        };
    }
}