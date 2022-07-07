using L_Commander.App.Infrastructure;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.ViewModels.Settings
{
    public class TagSettingsItemViewModel : ViewModelBase, ISettingsItemViewModel
    {
        private bool _isEnabled;
        private TagViewModel _selectedTag;

        public TagSettingsItemViewModel(TagSettings tagSettings)
        {
            if (tagSettings != null)
            {
                _isEnabled = tagSettings.IsEnabled;
            }

            Tags.Clear();
            foreach (var tag in tagSettings.Tags)
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

        public string DisplayName { get { return "Tag settings"; } }

        public ObservableCollection<TagViewModel> Tags { get; } = new ObservableCollection<TagViewModel>();

        public TagViewModel SelectedTag
        {
            get { return _selectedTag; }
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

        }

        private bool CanEditTagCommandHandler()
        {
            return SelectedTag != null;
        }

        private void EditTagCommandHandler()
        {

        }

        private bool CanDeleteCommandHandler()
        {
            return SelectedTag != null;
        }

        private void DeleteCommandHandler()
        {

        }
    }
}
