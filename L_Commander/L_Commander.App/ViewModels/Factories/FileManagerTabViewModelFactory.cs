using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels.Factories
{
    public interface IFileManagerTabViewModelFactory
    {
        IFileManagerTabViewModel CreateViewModel();
    }

    public sealed class FileManagerTabViewModelFactory : IFileManagerTabViewModelFactory
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IWindowManager _windowManager;
        private readonly IIconCache _iconCache;

        public FileManagerTabViewModelFactory(IFileSystemProvider fileSystemProvider, IWindowManager windowManager, IIconCache iconCache)
        {
            _fileSystemProvider = fileSystemProvider;
            _windowManager = windowManager;
            _iconCache = iconCache;
        }

        public IFileManagerTabViewModel CreateViewModel()
        {
            return new FileManagerTabViewModel(_fileSystemProvider, _windowManager);
        }
    }
}
