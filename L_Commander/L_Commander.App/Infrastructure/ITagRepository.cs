using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using L_Commander.App.Infrastructure.Db;
using L_Commander.App.OperatingSystem;
using Microsoft.EntityFrameworkCore;

namespace L_Commander.App.Infrastructure
{
    public interface ITagRepository
    {
        void Initialize();

        Tag[] GetTags(FileSystemEntryDescriptor descriptor);

        void SetTags(string path, Tag[] tags);
    }

    public sealed class TagRepository : ITagRepository
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly LCommanderDbContext _dbContext;

        public TagRepository(IFileSystemProvider fileSystemProvider, LCommanderDbContext dbContext)
        {
            _fileSystemProvider = fileSystemProvider;
            _dbContext = dbContext;
        }

        public void Initialize()
        {
            var fileEntities = _dbContext.Files.ToArray();
            foreach (var fileEntity in fileEntities)
            {
                if (!_fileSystemProvider.IsFileExists(fileEntity.Path))
                    _dbContext.Files.Remove(fileEntity);
            }

            _dbContext.SaveChanges();
        }

        public Tag[] GetTags(FileSystemEntryDescriptor descriptor)
        {
            if (!Exists(descriptor))
                return Array.Empty<Tag>();

            lock (_dbContext)
            {
                if (_dbContext.IsDisposing)
                    return Array.Empty<Tag>();

                var file = _dbContext.Files
                    .Where(x => x.Path == descriptor.Path)
                    .Include(x => x.Tags)
                    .FirstOrDefault();

                if (file == null)
                    return Array.Empty<Tag>();

                return file.Tags.Select(x => x.FromEntity()).ToArray();
            }
        }

        public void SetTags(string path, Tag[] tags)
        {
            var fileWithTags = _dbContext.Files.Find(path);
            if (fileWithTags != null)
            {
                foreach (var tag in tags)
                {
                    var tagEntity = _dbContext.Tags.Find(tag.Guid);
                    if (tagEntity != null)
                    {
                        tagEntity.Text = tag.Text;
                        tagEntity.Color = tag.Color;
                    }
                    else
                    {
                        tagEntity = tag.ToEntity();
                        fileWithTags.Tags.Add(tagEntity);

                        _dbContext.Tags.Add(tagEntity);
                    }
                }
            }
            else
            {
                fileWithTags = new FileEntity { Path = path };

                fileWithTags.Tags = tags.Select(x => x.ToEntity()).ToList();
                _dbContext.Add(fileWithTags);
            }

            _dbContext.SaveChanges();
        }

        private bool Exists(FileSystemEntryDescriptor descriptor)
        {
            if (descriptor.IsFile)
            {
                return _fileSystemProvider.IsFileExists(descriptor.Path);
            }

            return _fileSystemProvider.IsDirectoryExists(descriptor.Path);
        }
    }
}
