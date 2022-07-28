using L_Commander.App.Infrastructure;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using L_Commander.App.ViewModels.Factories;

namespace L_Commander.App.ViewModels.Settings
{
    public interface ISettingsViewModel : ISettingsFiller
    {
        void Initialize();
    }

    public interface ISettingsItemViewModel
    {
        string DisplayName { get; }

        bool IsChanged { get; }

        void Save(ClientSettings settings);
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ISettingsManager _settingManager;
        private readonly ISettingsItemsViewModelFactory _factory;
        private ISettingsItemViewModel _selectedSettingsItem;
        private ClientSettings _settings;

        public SettingsViewModel(ISettingsManager settingManager, ISettingsItemsViewModelFactory factory)
        {
            _settingManager = settingManager;
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
            _settings = _settingManager.Get();

            Items.Clear();
            foreach (var item in _factory.CreateSettingsItems(_settings))
            {
                Items.Add(item);
            }

            SelectedItem = Items.FirstOrDefault();
        }

        private bool CanOkCommandHandler()
        {
            return Items.Any(x => x.IsChanged);
        }

        public void FillSettings(ClientSettings settings)
        {
            foreach (var settingsItemViewModel in Items)
            {
                settingsItemViewModel.Save(settings);
            }
        }
    }
}