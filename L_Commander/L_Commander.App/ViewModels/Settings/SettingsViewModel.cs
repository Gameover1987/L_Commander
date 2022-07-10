using L_Commander.App.Infrastructure;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using L_Commander.App.ViewModels.Factories;

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

        bool IsChanged { get; }

        void Save(ClientSettings settings);
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ISettingsProvider _settingProvider;
        private readonly ISettingsItemsViewModelFactory _factory;
        private ISettingsItemViewModel _selectedSettingsItem;
        private ClientSettings _settings;

        public SettingsViewModel(ISettingsProvider settingProvider, ISettingsItemsViewModelFactory factory)
        {
            _settingProvider = settingProvider;
            _factory = factory;

            OkCommand = new DelegateCommand(x => { }, x => CanOkCommandHandler());
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

        public IDelegateCommand OkCommand { get; }

        public void Initialize()
        {
            _settings = _settingProvider.Get();

            Items.Clear();
            foreach (var item in _factory.CreateSettingsItems(_settings))
            {
                Items.Add(item);
            }

            SelectedItem = Items.FirstOrDefault();
        }

        public void Save()
        {
            foreach (var settingsItemViewModel in Items.Where(x => x.IsChanged))
            {
                settingsItemViewModel.Save(_settings);
            }

            _settingProvider.Save(_settings);
        }

        private bool CanOkCommandHandler()
        {
            return Items.Any(x => x.IsChanged);
        }
       
    }
}