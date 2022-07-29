using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;
using Newtonsoft.Json;

namespace L_Commander.App.ViewModels.Settings
{
    public class FilesAndFoldersAppearanceSettingsItemViewModel : ViewModelBase, ISettingsItemViewModel
    {
        private readonly FilesAndFoldersAppearanceSettings _settings;

        private bool _showSystemFiles;
        private bool _showHiddenFiles;


        public FilesAndFoldersAppearanceSettingsItemViewModel(FilesAndFoldersAppearanceSettings settings)
        {
            _settings = settings;
            _showSystemFiles = _settings.ShowSystemFilesAndFolders;
            _showHiddenFiles = _settings.ShowHiddenFilesAndFolders;
        }

        public string DisplayName => "Files and folders appearance";

        public bool IsChanged
        {
            get
            {
                var sourceJson = _settings.SerializeToJson(Formatting.Indented);
                var currentJson = GetSettings().SerializeToJson(Formatting.Indented);

                return sourceJson != currentJson;
            }
        }

        public bool ShowSystemFiles
        {
            get => _showSystemFiles;
            set
            {
                if (_showSystemFiles == value)
                    return;
                _showSystemFiles = value;
                OnPropertyChanged(() => ShowSystemFiles);
            }
        }

        public bool ShowHiddenFiles
        {
            get => _showHiddenFiles;
            set
            {
                if (_showHiddenFiles == value)
                    return;
                _showHiddenFiles = value;
                OnPropertyChanged(() => ShowSystemFiles);
            }
        }

        public void Save(ClientSettings settings)
        {
            settings.FilesAndFoldersAppearanceSettings = GetSettings();
        }

        private FilesAndFoldersAppearanceSettings GetSettings()
        {
            return new FilesAndFoldersAppearanceSettings
            {
                ShowHiddenFilesAndFolders = ShowHiddenFiles,
                ShowSystemFilesAndFolders = ShowSystemFiles
            };
        }
    }
}
