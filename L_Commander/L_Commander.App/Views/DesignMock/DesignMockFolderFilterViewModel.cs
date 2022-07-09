using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.Common.Extensions;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockFolderFilterViewModel : FolderFilterViewModel
    {
        public DesignMockFolderFilterViewModel()
        {
            var fileSystemProvider = new FileSystemProvider(new IconCache());

            var entries = fileSystemProvider
                .GetFileSystemEntries("E:\\Download")
                .Select(x => new FileSystemEntryViewModel(x, fileSystemProvider, new DesignMockExceptionHandler()))
                .Take(100)
                .ToArray()
                .ForEach(x => x.Initialize())
                .ToArray();

           Refresh(entries);
        }
    }
}
