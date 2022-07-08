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
            var settings = _settingProvider.Get();
            settings.TagSettings = new TagSettings();
            settings.TagSettings.Tags = new Tag[]
                {
                    new Tag()
                    {
                        Text = "Red",
                        Color = 16711680
                    },
                    new Tag()
                    {
                        Text = "Orange",
                        Color = 16753920
                    },
                    new Tag()
                    {
                        Text = "Yellow",
                        Color = Colors.Yellow.ToInt()
                    },
                    new Tag()
                    {
                        Text = "Green",
                        Color = 32768
                    },
                    new Tag()
                    {
                        Text = "Light blue",
                        Color = 11393254
                    },
                    new Tag()
                    {
                        Text = "Blue",
                        Color = 255
                    },
                    new Tag()
                    {
                        Text = "Violet",
                        Color = 15631086
                    },
                };

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

        private bool CanOkCommandHandler()
        {
            return true;
        }

        private static ISettingsItemViewModel[] GetSettingsItems(ClientSettings clientSettings)
        {
            return new ISettingsItemViewModel[]
            {
                new TagSettingsItemViewModel(clientSettings.TagSettings),
            };
        }        
    }
}