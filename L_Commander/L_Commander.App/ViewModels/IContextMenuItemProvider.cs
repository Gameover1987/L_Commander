using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels;

public interface IContextMenuItemProvider
{
    public ContextMenuItemViewModel[] GetMenuItems(IFileSystemEntryViewModel fileSystemEntry);
}