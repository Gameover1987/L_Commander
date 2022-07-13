using System;
using System.Windows.Media.Imaging;
using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels;

public interface IContextMenuItemProvider
{
    public ContextMenuItemViewModel[] GetMenuItems(FileSystemEntryDescriptor descriptor);
}

public class ContextMenuItemProvider : IContextMenuItemProvider
{
    public ContextMenuItemViewModel[] GetMenuItems(FileSystemEntryDescriptor descriptor)
    {
        return new ContextMenuItemViewModel[]
        {
            //new ContextMenuItemViewModel { DisplayName = "111" , Icon = GetApplicationIcon()},
            new ContextMenuItemViewModel { DisplayName = "222" },
            new ContextMenuItemViewModel { IsSeparator = true },
            new ContextMenuItemViewModel { DisplayName = "333" },
        };
    }

    private BitmapImage GetApplicationIcon()
    {
        return new BitmapImage(new Uri(
            "pack://application:,,,/L_Commander.App;component/L_logo.ico",
            UriKind.Absolute));
    }
}