using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.UI.Infrastructure;

namespace L_Commander.App.ViewModels.Factories
{
    public interface IFileManagerTabViewModelFactory
    {
        IFileManagerTabViewModel CreateFileManagerTab();
    }

    public class FileManagerTabViewModelFactory : IFileManagerTabViewModelFactory
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IWindowManager _windowManager;
        private readonly IProcessProvider _processProvider;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IFileSystemEntryViewModelFactory _fileSystemEntryViewModelFactory;
        private readonly ITagRepository _tagRepository;
        private readonly IOpenWithViewModel _openWithViewModel;

        public FileManagerTabViewModelFactory(IFileSystemProvider fileSystemProvider,
            IWindowManager windowManager,
            IProcessProvider processProvider, 
            IExceptionHandler exceptionHandler,
            IFileSystemEntryViewModelFactory fileSystemEntryViewModelFactory,
            ITagRepository tagRepository,
            IOpenWithViewModel openWithViewModel)
        {
            _fileSystemProvider = fileSystemProvider;
            _windowManager = windowManager;
            _processProvider = processProvider;
            _exceptionHandler = exceptionHandler;
            _fileSystemEntryViewModelFactory = fileSystemEntryViewModelFactory;
            _tagRepository = tagRepository;
            _openWithViewModel = openWithViewModel;
        }

        public IFileManagerTabViewModel CreateFileManagerTab()
        {
            var tab = new FileManagerTabViewModel(new FolderFilterViewModel(_tagRepository),
                _fileSystemProvider, 
                _windowManager, 
                _processProvider,
                _exceptionHandler, 
                new FolderWatcher(_fileSystemProvider), 
                new UiTimer(), 
                _fileSystemEntryViewModelFactory,
                _tagRepository,
                _openWithViewModel);
            return tab;
        }
    }
}
