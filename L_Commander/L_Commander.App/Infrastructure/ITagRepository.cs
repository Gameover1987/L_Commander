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

        void AddOrUpdateTag(Tag tag);

        void RemoveTag(Guid tagGuid);

        Tag[] GetAllTags();

        FileWithTags[] GetAllFilesWithTags();

        Tag[] GetTagsByPath(FileSystemEntryDescriptor descriptor);

        void SetTagsForPath(string path, Tag[] tags);
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

        public void AddOrUpdateTag(Tag tag)
        {
            var tagToUpdate = _dbContext.Tags.Find(tag.Guid);
            if (tagToUpdate != null)
            {
                tagToUpdate.Color = tag.Color;
                tagToUpdate.Text = tag.Text;
            }
            else
            {
                _dbContext.Tags.Add(tag.ToEntity());
            }

            _dbContext.SaveChanges();
        }

        public void RemoveTag(Guid tagGuid)
        {
            var tagToRemove = _dbContext.Tags.Find(tagGuid);
            if (tagToRemove == null)
                return;

            _dbContext.Tags.Remove(tagToRemove);
        }

        public Tag[] GetAllTags()
        {
            lock (_dbContext)
            {
                return _dbContext.Tags.Select(x => new Tag
                {
                    Guid = x.TagGuid,
                    Color = x.Color,
                    Text = x.Text,
                }).ToArray();
            }
        }

        public FileWithTags[] GetAllFilesWithTags()
        {
            lock (_dbContext)
            {
                var filesWithTags = _dbContext
                    .Files
                    .Include(x => x.Tags)
                    .ThenInclude(x => x.TagEntity)
                    .Where(x => x.Tags != null)
                    .Select(x => new FileWithTags
                    {
                        FilePath = x.Path,
                        Tags = x.Tags.Select(t => t.FromEntity()).ToArray()
                    })
                    .ToArray();
                return filesWithTags;
            }
        }

        public Tag[] GetTagsByPath(FileSystemEntryDescriptor descriptor)
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
                    .ThenInclude(x => x.TagEntity)
                    .FirstOrDefault();

                if (file == null)
                    return Array.Empty<Tag>();

                return file.Tags.Select(x => x.FromEntity()).ToArray();
            }
        }

        public void SetTagsForPath(string path, Tag[] tags)
        {
            var fileWithTags = _dbContext.Files.FirstOrDefault(x => x.Path == path);
            if (fileWithTags != null)
            {
                fileWithTags.Tags = tags.Select(x => new FileTagEntity
                {
                    FileEntity = fileWithTags,
                    TagEntity = _dbContext.Tags.First(t => t.TagGuid == x.Guid),
                }).ToList();
            }
            else
            {
                fileWithTags = new FileEntity { Path = path };
                fileWithTags.Tags = tags.Select(x => new FileTagEntity
                {
                    FileEntity = fileWithTags,
                    TagEntity = _dbContext.Tags.First(t => t.TagGuid == x.Guid),
                }).ToList();

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
