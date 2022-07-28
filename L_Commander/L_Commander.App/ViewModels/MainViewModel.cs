﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ControlzEx.Standard;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
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
        private readonly ISettingsManager _settingsManager;
        private readonly IFileManagerViewModel _leftFileManager;
        private readonly IFileManagerViewModel _rightFileManager;
        private IFileManagerViewModel _activeFileManager;
        private readonly ICopyOperation _copyOperation;
        private readonly IMoveOperation _moveOperation;
        private readonly IDeleteOperation _deleteOperation;
        private readonly IWindowManager _windowManager;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ISettingsViewModel _settingsViewModel;
        private readonly IFileSystemProvider _fileSystemProvider;
        private ProgressDialogController _progressDialogController;

        public MainViewModel(ISettingsManager settingsManager,
            IFileManagerViewModel leftFileManager,
            IFileManagerViewModel rightFileManager,
            ICopyOperation copyOperation,
            IMoveOperation moveOperation,
            IDeleteOperation deleteOperation,
            IWindowManager windowManager,
            IExceptionHandler exceptionHandler,
            ISettingsViewModel settingsViewModel,
            IFileSystemProvider fileSystemProvider)
        {
            _settingsManager = settingsManager;
            _leftFileManager = leftFileManager;
            _rightFileManager = rightFileManager;
            _copyOperation = copyOperation;
            _moveOperation = moveOperation;
            _deleteOperation = deleteOperation;
            _deleteOperation.Progress += FileSystemOperationOnProgress;
            _moveOperation.Progress += FileSystemOperationOnProgress;
            _copyOperation.Progress += FileSystemOperationOnProgress;
            _windowManager = windowManager;
            _exceptionHandler = exceptionHandler;
            _settingsViewModel = settingsViewModel;
            _fileSystemProvider = fileSystemProvider;

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

        public IFileManagerViewModel ActiveFileManager
        {
            get { return _activeFileManager; }
            set
            {
                if (_activeFileManager == value)
                    return;
                _activeFileManager = value;
                OnPropertyChanged(() => ActiveFileManager);
                OnPropertyChanged(() => AnotherFileManager);
            }
        }

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

        public IDelegateCommand DeleteCommand { get; }

        public IDelegateCommand MakeDirCommand { get; set; }

        public IDelegateCommand ShowSettingsCommand { get; }

        public void Initialize()
        {
            var settings = _settingsManager.Get();
            _leftFileManager.Initialize(settings?.LeftFileManagerSettings);
            _rightFileManager.Initialize(settings?.RightFileManagerSettings);

            _fileSystemProvider.Initialize();
        }

        public void FillSettings(ClientSettings settings)
        {
            settings.MainWindowSettings = new MainWindowSettings
            {
                Left = _windowManager.MainWindow.Left,
                Top = _windowManager.MainWindow.Top,
                Width = _windowManager.MainWindow.Width,
                Height = _windowManager.MainWindow.Height
            };
            settings.LeftFileManagerSettings = LeftFileManager.CollectSettings();
            settings.RightFileManagerSettings = RightFileManager.CollectSettings();
        }

        public MainWindowSettings GetMainWindowSettings()
        {
            return _settingsManager.Get()?.MainWindowSettings;
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


            return !_copyOperation.IsStarted;
        }

        private async void CopyCommandHandler()
        {
            try
            {
                var sourceEntries = ActiveFileManager?.SelectedTab.SelectedEntries
                    .Select(x => x.GetDescriptor())
                    .ToArray();
                if (await CheckSourceAndDestPath("Copy operation", sourceEntries, AnotherFileManager.SelectedTab.FullPath) == false)
                {
                    return;
                }

                var questionSettings = new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative };
                var result = await _windowManager.ShowQuestion("Copy operation", $"Do you want copy files to '{AnotherFileManager.SelectedTab.FullPath}'?", questionSettings);
                if (result != MessageDialogResult.Affirmative)
                    return;

                _progressDialogController = await _windowManager.ShowProgressDialog($"Copying files to \r\n'{AnotherFileManager.SelectedTab.FullPath}'", "Wait for copy...");
                _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;

                _copyOperation.Initialize(sourceEntries, AnotherFileManager.SelectedTab.FullPath);
                await _copyOperation.Execute();
            }
            catch (Exception exception)
            {
                _exceptionHandler.HandleExceptionWithMessageBox(exception);
            }
            finally
            {
                if (_progressDialogController != null)
                {
                    if (_progressDialogController.IsOpen)
                        await _progressDialogController.CloseAsync();
                    _progressDialogController.Canceled -= ProgressDialogControllerOnCanceled;
                }
            }
        }

        private async Task<bool> CheckSourceAndDestPath(string operationName, FileSystemEntryDescriptor[] sourceEntries, string destPath)
        {
            var sourcePaths = sourceEntries.Select(x => new Uri(x.Path)).ToArray();
            var destDir = new Uri(destPath);
            var metroDialogSettings = new MetroDialogSettings
            {
                DefaultButtonFocus = MessageDialogResult.Affirmative,
            };

            foreach (var sourcePath in sourcePaths)
            {
                if (sourcePath == destDir)
                {
                    await _windowManager.ShowMessage(operationName, $"Source path '{sourcePath.AbsolutePath}' and destination path '{destDir.AbsolutePath}' are equal!", metroDialogSettings);
                    return false;
                }

                if (sourcePath.IsBaseOf(destDir))
                {
                    await _windowManager.ShowMessage(operationName, $"Source path '{sourcePath.AbsolutePath}' is subpath of '{destDir.AbsolutePath}'!", metroDialogSettings);
                    return false;
                }
            }

            return true;
        }

        private bool CanMoveCommandHandler()
        {
            if (ActiveFileManager?.SelectedTab?.SelectedEntries.Any() == false)
                return false;
            if (AnotherFileManager?.SelectedTab == null)
                return false;

            return !_copyOperation.IsStarted;
        }

        private async void MoveCommandHandler()
        {
            try
            {
                var sourceEntries = ActiveFileManager?.SelectedTab.SelectedEntries
                   .Select(x => x.GetDescriptor())
                   .ToArray();

                if (await CheckSourceAndDestPath("Move operation", sourceEntries, AnotherFileManager.SelectedTab.FullPath) == false)
                {
                    return;
                }

                var questionSettings = new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative };
                var result = await _windowManager.ShowQuestion("Move operation", $"Do you want move files to '{AnotherFileManager.SelectedTab.FullPath}'?", questionSettings);
                if (result != MessageDialogResult.Affirmative)
                    return;

                _progressDialogController = await _windowManager.ShowProgressDialog($"Moving files to \r\n'{AnotherFileManager.SelectedTab.FullPath}'", "Wait for move...");
                _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;

                _moveOperation.Initialize(sourceEntries, AnotherFileManager.SelectedTab.FullPath);
                await _moveOperation.Execute();

                ActiveFileManager?.SelectedTab.ReLoad();
            }
            catch (Exception exception)
            {
                _exceptionHandler.HandleExceptionWithMessageBox(exception);
            }
            finally
            {
                if (_progressDialogController != null)
                {
                    if (_progressDialogController.IsOpen)
                        await _progressDialogController.CloseAsync();
                    _progressDialogController.Canceled -= ProgressDialogControllerOnCanceled;
                }
            }
        }

        private bool CanDeleteCommandHandler()
        {
            return ActiveFileManager?.SelectedTab?.SelectedEntries.Any() == true;
        }

        private async void DeleteCommandHandler()
        {
            try
            {
                var selectedEntries = ActiveFileManager.SelectedTab.SelectedEntries.ToArray();
                var titleMsg = $"Delete operation ({selectedEntries.Length}) items";

                var message = string.Join(Environment.NewLine, selectedEntries.Select(x => x.FullPath).Take(100).OrderBy(x => x));
                if (selectedEntries.Length > 50)
                {
                    var stringList = selectedEntries.Select(x => x.FullPath).Take(50).OrderBy(x => x).ToList();
                    stringList.Add("...");
                    stringList.Add("And other file system entries?");

                    message = string.Join(Environment.NewLine, stringList);
                }

                var dialogResult = await _windowManager.ShowQuestion(titleMsg, message);
                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                _progressDialogController = await _windowManager.ShowProgressDialog($"Deleting", "Wait for delete...");
                _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;

                _deleteOperation.Initialize(selectedEntries.Select(x => x.GetDescriptor()).ToArray());
                await _deleteOperation.Execute();

                ActiveFileManager?.SelectedTab.ReLoad();
            }
            catch (Exception exception)
            {
                _exceptionHandler.HandleExceptionWithMessageBox(exception);
            }
            finally
            {
                if (_progressDialogController != null)
                {
                    if (_progressDialogController.IsOpen)
                        await _progressDialogController.CloseAsync();
                    _progressDialogController.Canceled -= ProgressDialogControllerOnCanceled;
                }
            }
        }

        private bool CanMakeDirCommandHandler()
        {
            return ActiveFileManager?.SelectedTab?.MakeDirCommand.CanExecute() == true;
        }

        private void MakeDirCommandHandler()
        {
            ActiveFileManager?.SelectedTab?.MakeDirCommand.TryExecute();
        }

        private void ShowSettingsCommandHandler()
        {
            _settingsViewModel.Initialize();
            if (!_windowManager.ShowDialogWindow<SettingsWindow>(_settingsViewModel))
            {
                return;
            }

            _settingsManager.Save();
        }

        private void ProgressDialogControllerOnCanceled(object sender, EventArgs e)
        {
            if (_copyOperation.IsStarted)
                _copyOperation.Cancel();

            if (_moveOperation.IsStarted)
                _moveOperation.Cancel();

            if (_deleteOperation.IsStarted)
                _deleteOperation.Cancel();
        }

        private void FileSystemOperationOnProgress(object sender, OperationProgressEventArgs e)
        {
            ExecuteInUIThread(() =>
            {
                if (_progressDialogController == null)
                    return;

                _progressDialogController.Maximum = e.Total;
                _progressDialogController.SetMessage(e.CurrentItemName);
                _progressDialogController.SetProgress(e.Processed);
            });
        }
    }
}
