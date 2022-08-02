using System;
using System.Linq;
using System.Text;
using System.Threading;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels
{
    public interface IMultipleFilePropertiesViewModel
    {
        void Initialize(IFileSystemEntryViewModel[] entries);

        void Cancel();
    }

    public class MultipleFilePropertiesViewModel : ViewModelBase, IMultipleFilePropertiesViewModel
    {
        private const int MaxTitleLength = 50;

        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IExceptionHandler _exceptionHandler;

        private IFileSystemEntryViewModel[] _entries;

        private CancellationTokenSource _cancellationTokenSource;
        private string _title;
        private long _filesCount;
        private long _foldersCount;
        private string _sizeDescription;
        private string _errorDescription;
        private bool _isBusy;

        public MultipleFilePropertiesViewModel(IFileSystemProvider fileSystemProvider, IExceptionHandler exceptionHandler)
        {
            _fileSystemProvider = fileSystemProvider;
            _exceptionHandler = exceptionHandler;
        }

        public string Title
        {
            get { return _title; }
            private set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }

        public long FilesCount
        {
            get => _filesCount;
            private set
            {
                if (_filesCount == value)
                    return;
                _filesCount = value;
                OnPropertyChanged(() => FilesCount);
            }
        }

        public long FoldersCount
        {
            get => _foldersCount;
            private set
            {
                if (_foldersCount == value)
                    return;
                _foldersCount = value;
                OnPropertyChanged(() => FoldersCount);
            }
        }

        public string SizeDescription
        {
            get => _sizeDescription;
            private set
            {
                if (_sizeDescription == value)
                    return;
                _sizeDescription = value;
                OnPropertyChanged(() => SizeDescription);
            }
        }

        public string ErrorDescription
        {
            get { return _errorDescription; }
            private set
            {
                if (_errorDescription == value)
                    return;
                _errorDescription = value;
                OnPropertyChanged(() => ErrorDescription);
                OnPropertyChanged(() => HasErrors);
            }
        }

        public bool HasErrors => !_errorDescription.IsNullOrWhiteSpace();

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value)
                    return;
                _isBusy = value;
                OnPropertyChanged(() => IsBusy);
            }
        }

        public async void Initialize(IFileSystemEntryViewModel[] entries)
        {
            _entries = entries;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            Title = $"Properties: {SelectedEntriesToString(_entries)}";
            FilesCount = entries.Count(x => x.IsFile);
            FoldersCount = entries.Length - FilesCount;
            SizeDescription = string.Empty;

            try
            {
                IsBusy = true;
                await ThreadTaskExtensions.Run(() =>
                {
                    long totalSize = 0;
                    var errorsStringBuilder = new StringBuilder();
                    foreach (var entry in _entries)
                    {
                        try
                        {
                            if (token.IsCancellationRequested)
                                return;

                            if (entry.IsFile)
                            {
                                totalSize += entry.TotalSize;
                            }
                            else
                            {
                                var pathInfo = _fileSystemProvider.GetPathInfoRecursively(entry.FullPath);
                                totalSize += pathInfo.TotalSize;

                                FilesCount += pathInfo.FilesCount;
                                FoldersCount += pathInfo.FoldersCount;
                            }

                            SizeDescription = $"{totalSize.SizeAsString()} ({totalSize.ToStringSplitedBySpaces()})";
                        }
                        catch (Exception exception)
                        {
                            errorsStringBuilder.AppendLine(exception.Message);
                            ErrorDescription = errorsStringBuilder.ToString();
                        }
                    }
                }, token);
            }
            catch (Exception exception)
            {
                if (token.IsCancellationRequested)
                    return;

                _exceptionHandler.HandleExceptionWithMessageBox(exception);
            }
            finally
            {
                IsBusy = false;
                if (!token.IsCancellationRequested)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private string SelectedEntriesToString(IFileSystemEntryViewModel[] entries)
        {
            var str = string.Join(", ", entries.Select(x => x.Name));
            if (str.Length > MaxTitleLength)
            {
                return str.Substring(0, MaxTitleLength).TrimEnd(new[] { ',' }) + "...";
            }

            return str;
        }
    }
}
