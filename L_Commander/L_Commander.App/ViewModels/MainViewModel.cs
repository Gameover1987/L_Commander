using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.Views;

namespace L_Commander.App.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IFileManagerViewModel _leftFileManagerViewModel;
        private readonly IFileManagerViewModel _rightFileManagerViewModel;

        private IWindow _window;

        public MainViewModel(ISettingsProvider settingsProvider, IFileManagerViewModel leftFileManagerViewModel, IFileManagerViewModel rightFileManagerViewModel)
        {
            _settingsProvider = settingsProvider;
            _leftFileManagerViewModel = leftFileManagerViewModel;
            _rightFileManagerViewModel = rightFileManagerViewModel;
        }

        public IFileManagerViewModel LeftFileManagerViewModel => _leftFileManagerViewModel;

        public IFileManagerViewModel RightFileManagerViewModel => _rightFileManagerViewModel;

        public void Initialize(IWindow window)
        {
            _window = window;
            var settings = _settingsProvider.Get();
            _leftFileManagerViewModel.Initialize(settings?.LeftFileManagerSettings);
            _rightFileManagerViewModel.Initialize(settings?.RightFileManagerSettings);
        }

        public void SaveSettings()
        {
            var settings = new ClientSettings
            {
                MainWindowSettings = new MainWindowSettings
                {
                    Left = _window.Left,
                    Top = _window.Top, 
                    Width = _window.Width,
                    Height = _window.Height
                },
                LeftFileManagerSettings = LeftFileManagerViewModel.CollectSettings(),
                RightFileManagerSettings = RightFileManagerViewModel.CollectSettings()
            };

            _settingsProvider.Save(settings);
        }

        public MainWindowSettings GetMainWindowSettings()
        {
            return _settingsProvider.Get()?.MainWindowSettings;
        }
    }
}
