using L_Commander.App.Infrastructure;
using L_Commander.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.ViewModels.Settings
{
    public interface ISettingsViewModel
    {
        void Initialize();

        void Save();
    }

    public interface ISettingsItemViewModel
    {
        string DisplayName { get; }
    }

    public class SettingsItemViewModelBase : ViewModelBase
    {
        public string DisplayName { get; }
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ISettingsProvider _settingProvider;
        private ISettingsItemViewModel _selectedSettingsItem;

        public SettingsViewModel(ISettingsProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        public ObservableCollection<ISettingsItemViewModel> Items { get; } = new ObservableCollection<ISettingsItemViewModel>();

        public ISettingsItemViewModel SelectedItem
        {
            get { return _selectedSettingsItem; }
            set
            {
                if (_selectedSettingsItem == value)
                    return;
                _selectedSettingsItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }

        public void Initialize()
        {
            var settings = _settingProvider.Get();

            Items.Clear();
            foreach (var item in GetSettingsItems(settings))
            {
                Items.Add(item);
            }

            SelectedItem = Items.FirstOrDefault();
        }

        public void Save()
        {

        }

        private static ISettingsItemViewModel[] GetSettingsItems(ClientSettings clientSettings)
        {
            return new ISettingsItemViewModel[]
            {
                new TagSettingsItemViewModel(),
            };
        }
    }
}