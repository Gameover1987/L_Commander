﻿using L_Commander.App.Infrastructure;
using L_Commander.App.ViewModels.Settings;
using System.Linq;
using L_Commander.App.Infrastructure.Settings;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockTagSettingsItemViewModel : TagSettingsItemViewModel
    {
        public DesignMockTagSettingsItemViewModel()
            : base(new DesignMockTagRepository(), new DesignMockAddTagViewModel(), new DesignMockWindowManager())
        {
            SelectedTag = Tags.FirstOrDefault();
        }
    }

    internal sealed class DesignMockFilesAndFoldersSettingsItemViewModel : FilesAndFoldersSettingsItemViewModel
    {
        public DesignMockFilesAndFoldersSettingsItemViewModel() : base(new FilesAndFoldersSettings
        {
            ShowHiddenFilesAndFolders = true,
            ShowSystemFilesAndFolders = true
        })
        {
        }
    }
}