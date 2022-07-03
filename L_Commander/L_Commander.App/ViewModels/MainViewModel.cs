﻿using L_Commander.App.Infrastructure;
using L_Commander.App.Views;
using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IFileManagerViewModel _leftFileManager;
        private readonly IFileManagerViewModel _rightFileManager;

        private IWindow _window;

        public MainViewModel(ISettingsProvider settingsProvider, IFileManagerViewModel leftFileManager, IFileManagerViewModel rightFileManager)
        {
            _settingsProvider = settingsProvider;
            _leftFileManager = leftFileManager;
            _rightFileManager = rightFileManager;

            ActiveFileManager = LeftFileManager;

            RenameCommand = new DelegateCommand(RenameCommandHandler, CanRenameCommandHandler);
            OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
            CopyCommand = new DelegateCommand(CopyCommandHandler, CanCopyCommandHandler);
            MakeDirCommand = new DelegateCommand(MakeDirCommandHandler, CanMakeDirCommandHandler);
            DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);
        }

        public IFileManagerViewModel LeftFileManager => _leftFileManager;

        public IFileManagerViewModel RightFileManager => _rightFileManager;

        public IFileManagerViewModel ActiveFileManager { get; set; }

        public IDelegateCommand RenameCommand { get; }

        public IDelegateCommand OpenCommand { get; }

        public IDelegateCommand CopyCommand { get; }

        public IDelegateCommand MakeDirCommand { get; set; }

        public IDelegateCommand DeleteCommand { get; }

        public void Initialize(IWindow window)
        {
            _window = window;
            var settings = _settingsProvider.Get();
            _leftFileManager.Initialize(settings?.LeftFileManagerSettings);
            _rightFileManager.Initialize(settings?.RightFileManagerSettings);
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
                LeftFileManagerSettings = LeftFileManager.CollectSettings(),
                RightFileManagerSettings = RightFileManager.CollectSettings()
            };

            _settingsProvider.Save(settings);
        }

        public MainWindowSettings GetMainWindowSettings()
        {
            return _settingsProvider.Get()?.MainWindowSettings;
        }

        private bool CanRenameCommandHandler()
        {
            return ActiveFileManager?.SelectedTab?.RenameCommand.CanExecute() == true;
        }

        private void RenameCommandHandler()
        {
            ActiveFileManager?.SelectedTab?.RenameCommand.TryExecute();
        }

        private bool CanOpenCommandHandler()
        {
            return ActiveFileManager?.SelectedTab?.OpenCommand.CanExecute() == true;
        }

        private void OpenCommandHandler()
        {
            ActiveFileManager?.SelectedTab?.OpenCommand.TryExecute();
        }

        private bool CanCopyCommandHandler()
        {
            return ActiveFileManager?.SelectedTab?.CopyCommand.CanExecute() == true;
        }

        private void CopyCommandHandler()
        {
            ActiveFileManager?.SelectedTab?.CopyCommand.TryExecute();
        }

        private bool CanMakeDirCommandHandler()
        {
            return ActiveFileManager?.SelectedTab?.MakeDirCommand.CanExecute() == true;
        }

        private void MakeDirCommandHandler()
        {
            ActiveFileManager?.SelectedTab?.MakeDirCommand.TryExecute();
        }

        private bool CanDeleteCommandHandler()
        {
            return ActiveFileManager?.SelectedTab?.DeleteCommand.CanExecute() == true;
        }

        private void DeleteCommandHandler()
        {
            ActiveFileManager?.SelectedTab?.DeleteCommand.TryExecute();
        }
    }
}
