using System;
using System.Linq;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem.Operations;
using L_Commander.App.ViewModels.Settings;
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
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ISettingsViewModel _settingsViewModel;
        private ProgressDialogController _progressDialogController;

        private IWindow _window;

        public MainViewModel(ISettingsProvider settingsProvider,
            IFileManagerViewModel leftFileManager,
            IFileManagerViewModel rightFileManager,
            ICopyOperation copyOperation,
            IWindowManager windowManager,
            IExceptionHandler exceptionHandler,
            ISettingsViewModel settingsViewModel)
        {
            _settingsProvider = settingsProvider;
            _leftFileManager = leftFileManager;
            _rightFileManager = rightFileManager;
            _copyOperation = copyOperation;
            _copyOperation.Progress += CopyOperationOnProgress;
            _windowManager = windowManager;
            _exceptionHandler = exceptionHandler;
            _settingsViewModel = settingsViewModel;
            ActiveFileManager = LeftFileManager;

            RenameCommand = new DelegateCommand(RenameCommandHandler, CanRenameCommandHandler);
            OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
            CopyCommand = new DelegateCommand(CopyCommandHandler, CanCopyCommandHandler);
            MoveCommand = new DelegateCommand(MoveCommandHandler, CanMoveCommandHandler);
            MakeDirCommand = new DelegateCommand(MakeDirCommandHandler, CanMakeDirCommandHandler);
            DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);
            ShowSettingsCommand = new DelegateCommand(ShowSettingsCommandHandler);
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

        public IDelegateCommand MoveCommand { get; }

        public IDelegateCommand MakeDirCommand { get; set; }

        public IDelegateCommand DeleteCommand { get; }

        public IDelegateCommand ShowSettingsCommand { get; }

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
            try
            {
                var sourceEntries = ActiveFileManager?.SelectedTab.SelectedEntries
                    .Select(x => x.GetDescriptor())
                    .ToArray();

                var questionSettings = new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative };
                var result = await _windowManager.ShowQuestion("Copy operation", $"Do you want copy files to '{AnotherFileManager.SelectedTab.FullPath}'?", questionSettings);
                if (result != MessageDialogResult.Affirmative)
                    return;

                _progressDialogController = await _windowManager.ShowProgressDialog($"Copying files to \r\n'{AnotherFileManager.SelectedTab.FullPath}'", "Wait for copy...");
                _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;

                await _copyOperation.Execute(sourceEntries, AnotherFileManager.SelectedTab.FullPath);

                await _progressDialogController.CloseAsync();
            }
            catch (Exception exception)
            {
                _exceptionHandler.HandleExceptionWithMessageBox(exception);
            }
            finally
            {
                if (_progressDialogController != null)
                    _progressDialogController.Canceled -= ProgressDialogControllerOnCanceled;
            }
        }

        private bool CanMoveCommandHandler()
        {
            if (ActiveFileManager?.SelectedTab?.SelectedEntries.Any() == false)
                return false;
            if (AnotherFileManager?.SelectedTab == null)
                return false;

            return !_copyOperation.IsBusy;
        }

        private async void MoveCommandHandler()
        {
            try
            {

                var sourceEntries = ActiveFileManager?.SelectedTab.SelectedEntries
                   .Select(x => x.GetDescriptor())
                   .ToArray();

                var questionSettings = new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative };
                var result = await _windowManager.ShowQuestion("Move operation", $"Do you want move files to '{AnotherFileManager.SelectedTab.FullPath}'?", questionSettings);
                if (result != MessageDialogResult.Affirmative)
                    return;

                _progressDialogController = await _windowManager.ShowProgressDialog($"Moving files to \r\n'{AnotherFileManager.SelectedTab.FullPath}'", "Wait for move...");
                _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;

                await _copyOperation.Execute(sourceEntries, AnotherFileManager.SelectedTab.FullPath, cleanupSourceEntries: true);

                await _progressDialogController.CloseAsync();
            }
            catch (Exception exception)
            {
                _exceptionHandler.HandleExceptionWithMessageBox(exception);
            }
            finally
            {
                if (_progressDialogController != null)
                    _progressDialogController.Canceled -= ProgressDialogControllerOnCanceled;
            }
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

        private void ShowSettingsCommandHandler()
        {
            _settingsViewModel.Initialize();
            if (!_windowManager.ShowDialogWindow<SettingsWindow>(_settingsViewModel))
            {
                return;
            }

            _settingsViewModel.Save();
        }
    }
}
