using System;
using System.Diagnostics;
using System.Threading;
using L_Commander.App.OperatingSystem;

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
}
