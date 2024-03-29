﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.Common.Extensions;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockTagRepository : ITagRepository
    {
        private readonly Tag[] _tags = new Tag[]
        {
            new Tag { Color = Colors.Red.ToInt() },
            new Tag { Color = Colors.Orange.ToInt() },
            new Tag { Color = Colors.Yellow.ToInt() },
            new Tag { Color = Colors.Green.ToInt() },
            new Tag { Color = Colors.LightBlue.ToInt() },
            new Tag { Color = Colors.Blue.ToInt() },
            new Tag { Color = Colors.Violet.ToInt() }
        };

        public void Initialize()
        {
            
        }

        public void AddOrUpdateTag(Tag tag)
        {
            throw new NotImplementedException();
        }

        public void RemoveTag(Guid tagGuid)
        {
            throw new NotImplementedException();
        }

        public Tag[] GetAllTags()
        {
            return _tags;
        }

        public FileWithTags[] GetAllFilesWithTags()
        {
            return new FileWithTags[0];
        }

        public Tag[] GetTagsByPath(FileSystemEntryDescriptor descriptor)
        {
            var random = new Random(DateTime.Now.Millisecond);
            return _tags.Take(random.Next(1, _tags.Length)).ToArray();
        }

        public void SetTagsForPath(string path, Tag[] tags)
        {
            
        }
    }

    internal sealed class DesignMockFolderFilterViewModel : FolderFilterViewModel
    {
        public DesignMockFolderFilterViewModel() 
            : base(new DesignMockTagRepository())
        {
            var fileSystemProvider = new FileSystemProvider(new IconCache());

            var entries = fileSystemProvider
                .GetFileSystemEntries("E:\\Download")
                .Select(x => new FileSystemEntryViewModel(x, fileSystemProvider, new DesignMockExceptionHandler(), new DesignMockTagRepository()))
                .Take(100)
                .ToArray()
                .ForEach(x => x.Initialize())
                .ToArray();

           Refresh(entries);
        }
    }
}
