using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.App.OperatingSystem;
using L_Commander.App.OperatingSystem.Operations;
using L_Commander.App.ViewModels.Factories;
using L_Commander.App.ViewModels.History;
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
        private readonly IHistoryViewModel _historyViewModel;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ISettingsViewModel _settingsViewModel;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IFileSystemEntryViewModelFactory _factory;

        private ProgressDialogController _progressDialogController;

        public MainViewModel(ISettingsManager settingsManager,
            IFileManagerViewModel leftFileManager,
            IFileManagerViewModel rightFileManager,
            ICopyOperation copyOperation,
            IMoveOperation moveOperation,
            IDeleteOperation deleteOperation,
            IWindowManager windowManager,
            IHistoryViewModel historyViewModel,
            IExceptionHandler exceptionHandler,
            ISettingsViewModel settingsViewModel,
            IFileSystemProvider fileSystemProvider,
            IFileSystemEntryViewModelFactory factory)
        {
            _settingsManager = settingsManager;
            _leftFileManager = leftFileManager;
            _rightFileManager = rightFileManager;
            _copyOperation = copyOperation;
            _moveOperation = moveOperation;
            _deleteOperation = deleteOperation;
            _deleteOperation.TotalProgress += FileSystemOperationOnProgress;
            _moveOperation.TotalProgress += FileSystemOperationOnProgress;
            _moveOperation.ActiveItemsProgress += CopyOperationOnActiveItemsProgress;
            _copyOperation.TotalProgress += FileSystemOperationOnProgress;
            _copyOperation.ActiveItemsProgress += CopyOperationOnActiveItemsProgress;
            _windowManager = windowManager;
            _historyViewModel = historyViewModel;
            _exceptionHandler = exceptionHandler;
            _settingsViewModel = settingsViewModel;
            _fileSystemProvider = fileSystemProvider;
            _factory = factory;

            ActiveFileManager = LeftFileManager;

            RenameCommand = new DelegateCommand(RenameCommandHandler, CanRenameCommandHandler);
            OpenCommand = new DelegateCommand(OpenCommandHandler, CanOpenCommandHandler);
            CopyCommand = new DelegateCommand(CopyCommandHandler, CanCopyCommandHandler);
            MoveCommand = new DelegateCommand(MoveCommandHandler, CanMoveCommandHandler);
            MakeDirCommand = new DelegateCommand(MakeDirCommandHandler, CanMakeDirCommandHandler);
            DeleteCommand = new DelegateCommand(DeleteCommandHandler, CanDeleteCommandHandler);
            ShowSettingsCommand = new DelegateCommand(ShowSettingsCommandHandler);
            ShowHistoryCommand = new DelegateCommand(ShowHistoryCommandHandler);
            NavigateToFavoriteCommand = new DelegateCommand(NavigateToFavoriteCommandHandler);
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

        public ObservableCollection<IFavoriteFileSystemEntryViewModel> Favorites { get; } = new ObservableCollection<IFavoriteFileSystemEntryViewModel>();

        public IDelegateCommand RenameCommand { get; }

        public IDelegateCommand OpenCommand { get; }

        public IDelegateCommand CopyCommand { get; }

        public IDelegateCommand MoveCommand { get; }

        public IDelegateCommand DeleteCommand { get; }

        public IDelegateCommand MakeDirCommand { get; set; }

        public IDelegateCommand ShowSettingsCommand { get; }

        public IDelegateCommand ShowHistoryCommand { get; }

        public IDelegateCommand NavigateToFavoriteCommand { get; }

        public void Initialize()
        {
            var settings = _settingsManager.Get();
            _leftFileManager.Initialize(settings?.LeftFileManagerSettings);
            _rightFileManager.Initialize(settings?.RightFileManagerSettings);

            _fileSystemProvider.Initialize();

            Favorites.Clear();
            if (settings.FavoritesSettings != null)
            {

            }
            else
            {
                foreach (var specialFolderPath in GetDefaultFavorites())
                {
                    Favorites.Add(_factory.CreateFavorite(specialFolderPath));
                }
            }
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
            var descriptors = ActiveFileManager?.SelectedTab.SelectedEntries
                .Select(x => x.GetDescriptor())
                .ToArray();

            await Copy(descriptors, AnotherFileManager.SelectedTab.FullPath);
        }

        public async Task Copy(FileSystemEntryDescriptor[] descriptors, string destPath)
        {
            try
            {
                if (await CheckSourceAndDestPath("Copy operation", descriptors, destPath) == false)
                {
                    return;
                }

                var message = GetFilesListMessage(descriptors);

                var questionSettings = new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative };
                var result = await _windowManager.ShowQuestion($"Copy ({descriptors.Length}) items to '{destPath}'", message, questionSettings);
                if (result != MessageDialogResult.Affirmative)
                    return;

                _progressDialogController = await _windowManager.ShowProgressDialog(
                    $"Copying files to \r\n'{destPath}'",
                    "Wait for copy...");
                _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;

                _copyOperation.Initialize(descriptors, destPath);
                await _copyOperation.Execute();

                if (_copyOperation.HasErrors)
                {
                    var aggregateException = new AggregateException(_deleteOperation.Errors);
                    _exceptionHandler.HandleExceptionWithMessageBox(aggregateException);
                }
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

        private string GetFilesListMessage(FileSystemEntryDescriptor[] descriptors)
        {
            var message = string.Join(Environment.NewLine, descriptors.Select(x => x.Path).Take(100).OrderBy(x => x));
            if (descriptors.Length > 50)
            {
                var stringList = descriptors.Select(x => x.Path).Take(50).OrderBy(x => x).ToList();
                stringList.Add("...");
                stringList.Add("And other file system entries?");

                message = string.Join(Environment.NewLine, stringList);
            }

            return message;
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
            var sourceEntries = ActiveFileManager?.SelectedTab.SelectedEntries
               .Select(x => x.GetDescriptor())
               .ToArray();

            await Move(sourceEntries, AnotherFileManager.SelectedTab.FullPath);
            ActiveFileManager.SelectedTab.ReLoad();
        }

        public async Task Move(FileSystemEntryDescriptor[] descriptors, string destPath)
        {
            try
            {
                if (await CheckSourceAndDestPath("Move operation", descriptors, destPath) == false)
                {
                    return;
                }

                var questionSettings = new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative };
                var result = await _windowManager.ShowQuestion($"Move ({descriptors.Length}) items to '{destPath}'", GetFilesListMessage(descriptors), questionSettings);
                if (result != MessageDialogResult.Affirmative)
                    return;

                _progressDialogController = await _windowManager.ShowProgressDialog($"Moving files to \r\n'{destPath}'", "Wait for move...");
                _progressDialogController.Canceled += ProgressDialogControllerOnCanceled;

                _moveOperation.Initialize(descriptors, destPath);
                await _moveOperation.Execute();

                if (_moveOperation.HasErrors)
                {
                    var aggregateException = new AggregateException(_moveOperation.Errors);
                    _exceptionHandler.HandleExceptionWithMessageBox(aggregateException);
                }
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

                if (_deleteOperation.HasErrors)
                {
                    var aggregateException = new AggregateException(_deleteOperation.Errors);
                    _exceptionHandler.HandleExceptionWithMessageBox(aggregateException);
                }

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

        private void ShowHistoryCommandHandler()
        {
            _historyViewModel.Initialize();
            _windowManager.ShowDialogWindow<HistoryWindow>(_historyViewModel);
        }

        private void NavigateToFavoriteCommandHandler(object obj)
        {
            if (ActiveFileManager.SelectedTab == null)
                return;

            var favorite = (IFavoriteFileSystemEntryViewModel)obj;
            ActiveFileManager.SelectedTab.FullPath = favorite.FullPath;
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

                if (_deleteOperation.IsStarted)
                {
                    _progressDialogController.SetMessage(e.CurrentItemName);
                }

                _progressDialogController.SetProgress(e.Processed);
            });
        }


        private void CopyOperationOnActiveItemsProgress(object sender, CopyProgressEventArgs e)
        {
            ExecuteInUIThread(() =>
            {
                if (_progressDialogController == null)
                    return;

                var isMoving = sender is IMoveOperation;

                var progressMessage = GetProgressMessage(e.ActiveWorks, isMoving);
                _progressDialogController.SetMessage(progressMessage);
            });
        }

        private string GetProgressMessage(CopyUnitOfWork[] works, bool isMoving)
        {
            var builder = new StringBuilder();
            foreach (var work in works)
            {
                builder.AppendLine($"Copying {work.Percent}% - {work.DestinationPath}");
            }

            return builder.ToString();
        }

        private string[] GetDefaultFavorites()
        {
            return new string[]
            {
                _fileSystemProvider.GetSpecialFolderPath(Environment.SpecialFolder.Desktop),
                _fileSystemProvider.GetSpecialFolderPath(Environment.SpecialFolder.MyDocuments),
                _fileSystemProvider.GetSpecialFolderPath(Environment.SpecialFolder.MyMusic),
                _fileSystemProvider.GetSpecialFolderPath(Environment.SpecialFolder.MyVideos),
                _fileSystemProvider.GetSpecialFolderPath(Environment.SpecialFolder.MyPictures),
            }
            .Where(x => x != null)
            .ToArray();
        }
    }
}
