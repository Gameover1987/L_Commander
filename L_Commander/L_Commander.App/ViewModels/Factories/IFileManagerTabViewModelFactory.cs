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
        private readonly IOperatingSystemProvider _operatingSystemProvider;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IFileSystemEntryViewModelFactory _fileSystemEntryViewModelFactory;
        private readonly ITagRepository _tagRepository;

        public FileManagerTabViewModelFactory(IFileSystemProvider fileSystemProvider,
            IWindowManager windowManager,
            IOperatingSystemProvider operatingSystemProvider, 
            IExceptionHandler exceptionHandler,
            IFileSystemEntryViewModelFactory fileSystemEntryViewModelFactory,
            ITagRepository tagRepository)
        {
            _fileSystemProvider = fileSystemProvider;
            _windowManager = windowManager;
            _operatingSystemProvider = operatingSystemProvider;
            _exceptionHandler = exceptionHandler;
            _fileSystemEntryViewModelFactory = fileSystemEntryViewModelFactory;
            _tagRepository = tagRepository;
        }

        public IFileManagerTabViewModel CreateFileManagerTab()
        {
            var tab = new FileManagerTabViewModel(new FolderFilterViewModel(_tagRepository),
                _fileSystemProvider, 
                _windowManager, 
                _operatingSystemProvider,
                _exceptionHandler, 
                new FolderWatcher(_fileSystemProvider), 
                new UiTimer(), 
                _fileSystemEntryViewModelFactory,
                _tagRepository);
            return tab;
        }
    }
}
