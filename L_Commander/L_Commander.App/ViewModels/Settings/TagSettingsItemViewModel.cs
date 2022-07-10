using L_Commander.App.Infrastructure;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using L_Commander.App.Views;
using Newtonsoft.Json;

namespace L_Commander.App.ViewModels.Settings
{
    public class TagSettingsItemViewModel : ViewModelBase, ISettingsItemViewModel
    {
        private readonly TagSettings _tagSettings;
        private readonly IAddTagViewModel _addTagViewModel;
        private readonly IWindowManager _windowManager;
        private bool _isEnabled;
        private TagViewModel _selectedTag;

        public TagSettingsItemViewModel(TagSettings tagSettings, IAddTagViewModel addTagViewModel, IWindowManager windowManager)
        {
            _tagSettings = tagSettings ?? new TagSettings { Tags = Array.Empty<Tag>() };
            _addTagViewModel = addTagViewModel;
            _windowManager = windowManager;

            Tags.Clear();
            _isEnabled = _tagSettings.IsEnabled;
            foreach (var tag in _tagSettings.Tags)
            {
                Tags.Add(new TagViewModel(tag));
            }

            AddTagCommand = new DelegateCommand(AddTagCommandHandler);
            EditTagCommand = new DelegateCommand(EditTagCommandHandler, CanEditTagCommandHandler);
            DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                    return;

                _isEnabled = value;
                OnPropertyChanged(() => IsEnabled);
            }
        }

        public string DisplayName => "Tag settings";

        public bool IsChanged
        {
            get
            {
                if (_tagSettings.IsEnabled != IsEnabled)
                    return true;

                if (_tagSettings.Tags.Length != Tags.Count)
                    return true;

                var sourceJson = JsonConvert.SerializeObject(_tagSettings.Tags, Formatting.Indented);
                var currentJson = JsonConvert.SerializeObject(Tags.Select(x => x.GetTag()).ToArray());

                return sourceJson != currentJson;
            }
        }

        public void Save(ClientSettings settings)
        {
            settings.TagSettings = new TagSettings();
            settings.TagSettings.IsEnabled = true;
            settings.TagSettings.Tags = Tags.Select(x => x.GetTag()).ToArray();
        }

        public ObservableCollection<TagViewModel> Tags { get; } = new ObservableCollection<TagViewModel>();

        public TagViewModel SelectedTag
        {
            get => _selectedTag;
            set
            {
                if (_selectedTag == value)
                    return;
                _selectedTag = value;
                OnPropertyChanged(() => SelectedTag);
            }
        }

        public IDelegateCommand AddTagCommand { get; }

        public IDelegateCommand EditTagCommand { get; }

        public IDelegateCommand DeleteCommand { get; }

        private void AddTagCommandHandler()
        {
            _addTagViewModel.Initialize(null);
            if (!_windowManager.ShowDialogWindow<AddTagWindow>(_addTagViewModel))
                return;

            var tag = _addTagViewModel.GetTag();
            Tags.Add(new TagViewModel(tag));
        }

        private bool CanEditTagCommandHandler()
        {
            return SelectedTag != null;
        }

        private void EditTagCommandHandler()
        {
            _addTagViewModel.Initialize(SelectedTag.GetTag());
            if (!_windowManager.ShowDialogWindow<AddTagWindow>(_addTagViewModel))
                return;

            var tag = _addTagViewModel.GetTag();
            SelectedTag.Update(tag);
        }

        private bool CanDeleteCommandHandler()
        {
            return SelectedTag != null;
        }

        private void DeleteCommandHandler()
        {
            Tags.Remove(SelectedTag);
        }
    }
}
