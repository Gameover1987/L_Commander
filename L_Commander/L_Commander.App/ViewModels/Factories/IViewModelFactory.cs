using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.UI.Infrastructure;

namespace L_Commander.App.ViewModels.Factories
{
    public interface IViewModelFactory
    {
        IFileManagerTabViewModel CreateFileManagerTab();
    }

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IWindowManager _windowManager;
        private readonly IOperatingSystemProvider _operatingSystemProvider;
        private readonly IExceptionHandler _exceptionHandler;

        public ViewModelFactory(IFileSystemProvider fileSystemProvider, IWindowManager windowManager, IOperatingSystemProvider operatingSystemProvider, IExceptionHandler exceptionHandler)
        {
            _fileSystemProvider = fileSystemProvider;
            _windowManager = windowManager;
            _operatingSystemProvider = operatingSystemProvider;
            _exceptionHandler = exceptionHandler;
        }

        public IFileManagerTabViewModel CreateFileManagerTab()
        {
            var tab = new FileManagerTabViewModel(new FolderFilterViewModel(), _fileSystemProvider, _windowManager, _operatingSystemProvider, _exceptionHandler, new FolderWatcher(), new UiTimer());
            return tab;
        }
    }
}
