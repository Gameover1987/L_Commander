using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockContextMenuItemProvider : IContextMenuItemProvider
    {
        public ContextMenuItemViewModel[] GetMenuItems(FileSystemEntryDescriptor descriptor)
        {
            return new ContextMenuItemViewModel[0];
        }
    }
}
