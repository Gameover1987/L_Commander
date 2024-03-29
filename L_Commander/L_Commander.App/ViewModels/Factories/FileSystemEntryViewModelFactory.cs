﻿using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;

namespace L_Commander.App.ViewModels.Factories
{
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

        public IFileSystemEntryViewModel CreateEntryViewModel(FileSystemEntryDescriptor descriptor)
        {
            return new FileSystemEntryViewModel(descriptor, _fileSystemProvider, _exceptionHandler, _tagRepository);
        }

        public IFavoriteFileSystemEntryViewModel CreateFavorite(string path)
        {
            return new FavoriteFileSystemEntryViewModel(_fileSystemProvider.GetFileSystemDescriptor(path));
        }

        public IMultipleFilePropertiesViewModel CreateMultiplePropertiesViewModel()
        {
            return new MultipleFilePropertiesViewModel(_fileSystemProvider, _exceptionHandler);
        }
    }
}
