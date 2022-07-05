using System;
using System.Linq;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem.Operations;
using L_Commander.App.Views;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IFileManagerViewModel _leftFileManager;
        private readonly IFileManagerViewModel _rightFileManager;
        private readonly ICopyOperation _copyOperation;
        private readonly IWindowManager _windowManager;

        private ProgressDialogController _progressDialogController;

        private IWindow _window;

        public MainViewModel(ISettingsProvider settingsProvider, IFileManagerViewModel leftFileManager, IFileManagerViewModel rightFileManager, ICopyOperation copyOperation, IWindowManager windowManager)
        {
            _settingsProvider = settingsProvider;
            _leftFileManager = leftFileManager;
            _rightFileManager = rightFileManager;
            _copyOperation = copyOperation;
            _windowManager = windowManager;

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

        public IFileManagerViewModel AnotherFileManager
        {
            get
            {
                if (ActiveFileManager == null)
                    return null;

                if (ActiveFileManager == LeftFileManager)
                    return RightFileManager;

                return LeftFileManager;
            }
        }

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
            if (ActiveFileManager?.SelectedTab?.SelectedEntries.Any() == false)
                return false;
            if (AnotherFileManager?.SelectedTab == null)
                return false;

            return !_copyOperation.IsBusy;
        }

        private async void CopyCommandHandler()
        {
            var sourceEntries = ActiveFileManager?.SelectedTab.SelectedEntries
                .Select(x => x.GetDescriptor())
                .ToArray();

            _progressDialogController = await _windowManager.ShowProgressDialog($"Copying files to \r\n'{AnotherFileManager.SelectedTab.FullPath}'", "Wait for copy");
            _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;
            _copyOperation.Progress += CopyOperationOnProgress;
            await _copyOperation.Execute(sourceEntries, AnotherFileManager.SelectedTab.FullPath);

            await _progressDialogController.CloseAsync();
        }

        private void ProgressDialogControllerOnCanceled(object sender, EventArgs e)
        {
            _copyOperation.Cancel();
        }

        private void CopyOperationOnProgress(object sender, CopyProgressEventArgs e)
        {
            ExecuteInUIThread(() =>
            {
                if (_progressDialogController == null)
                    return;

                _progressDialogController.Maximum = e.Total;
                _progressDialogController.SetMessage(e.CurrentFileName);
                _progressDialogController.SetProgress(e.Copied);
            });
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
