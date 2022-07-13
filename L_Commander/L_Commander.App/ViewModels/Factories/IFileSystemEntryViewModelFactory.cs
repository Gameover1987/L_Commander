using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels.Factories
{
    public interface IFileSystemEntryViewModelFactory
    {
        public IFileSystemEntryViewModel CreateEntryViewModel(string path);
    }

    public class FileSystemEntryViewModelFactory : IFileSystemEntryViewModelFactory
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IContextMenuItemProvider _contextMenuItemProvider;

        public FileSystemEntryViewModelFactory(IFileSystemProvider fileSystemProvider, IExceptionHandler exceptionHandler, IContextMenuItemProvider contextMenuItemProvider)
        {
            _fileSystemProvider = fileSystemProvider;
            _exceptionHandler = exceptionHandler;
            _contextMenuItemProvider = contextMenuItemProvider;
        }

        public IFileSystemEntryViewModel CreateEntryViewModel(string path)
        {
            return new FileSystemEntryViewModel(path, _fileSystemProvider, _exceptionHandler, _contextMenuItemProvider);
        }
    }
}
