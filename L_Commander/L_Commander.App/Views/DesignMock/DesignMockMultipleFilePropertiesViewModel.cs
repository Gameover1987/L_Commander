using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using L_Commander.Common.Extensions;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockMultipleFilePropertiesViewModel : MultipleFilePropertiesViewModel
    {
        public DesignMockMultipleFilePropertiesViewModel()
            : base(new FileSystemProvider(new IconCache()), new DesignMockExceptionHandler())
        {
            ThreadTaskExtensions.IsSyncRun = true;
            
            var fileSystemProvider  = new FileSystemProvider(new IconCache());

            Initialize(new []
            {
                new FileSystemEntryViewModel( fileSystemProvider.GetFileSystemDescriptor("E:\\Download"), new FileSystemProvider(new IconCache()), new DesignMockExceptionHandler(), new DesignMockTagRepository()),
                new FileSystemEntryViewModel(fileSystemProvider.GetFileSystemDescriptor("E:\\Stacy"), new FileSystemProvider(new IconCache()), new DesignMockExceptionHandler(), new DesignMockTagRepository())
            });
        }
    }
}
