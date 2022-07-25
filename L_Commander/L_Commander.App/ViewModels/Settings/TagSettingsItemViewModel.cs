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
        private readonly ITagRepository _tagRepository;
        private readonly IAddTagViewModel _addTagViewModel;
        private readonly IWindowManager _windowManager;
        
        private TagViewModel _selectedTag;
        private Tag[] _sourceTags;

        public TagSettingsItemViewModel(ITagRepository tagRepository, IAddTagViewModel addTagViewModel, IWindowManager windowManager)
        {
            _tagRepository = tagRepository;
            _addTagViewModel = addTagViewModel;
            _windowManager = windowManager;

            Tags.Clear();
            _sourceTags = _tagRepository.GetAllTags();
            foreach (var tag in _sourceTags)
            {
                Tags.Add(new TagViewModel(tag));
            }

            AddTagCommand = new DelegateCommand(AddTagCommandHandler);
            EditTagCommand = new DelegateCommand(EditTagCommandHandler, CanEditTagCommandHandler);
            DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);
        }

        public string DisplayName => "Tag settings";

        public bool IsChanged
        {
            get
            {
                if (_sourceTags.Length != Tags.Count)
                    return true;

                var sourceJson = JsonConvert.SerializeObject(_sourceTags, Formatting.Indented);
                var currentJson = JsonConvert.SerializeObject(Tags.Select(x => x.GetTag()).ToArray());

                return sourceJson != currentJson;
            }
        }

        public void Save()
        {
            foreach (var tagViewModel in Tags)
            {
                _tagRepository.AddOrUpdateTag(tagViewModel.GetTag());
            }
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
