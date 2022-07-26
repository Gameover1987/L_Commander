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
        public IFileSystemEntryViewModel CreateEntryViewModel(string path, IFileManagerTabViewModel tab);
    }

    public class FileSystemEntryViewModelFactory : IFileSystemEntryViewModelFactory
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ITagRepository _tagRepository;

        public FileSystemEntryViewModelFactory(IFileSystemProvider fileSystemProvider, IExceptionHandler exceptionHandler, ITagRepository tagRepository)
        {
            _fileSystemProvider = fileSystemProvider;
            _exceptionHandler = exceptionHandler;
            _tagRepository = tagRepository;
        }

        public IFileSystemEntryViewModel CreateEntryViewModel(string path, IFileManagerTabViewModel tab)
        {
            return new FileSystemEntryViewModel(path, _fileSystemProvider, _exceptionHandler, _tagRepository);
        }
    }
}
