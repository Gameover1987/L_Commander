using System;
using System.IO;
using System.Threading;

namespace L_Commander.App.OperatingSystem
{
    public enum FileChangeType
    {
        Create,
        Rename,
        Delete,
        Other
    }

    public class FileChangedEventArgs
    {
        public FileChangedEventArgs(FileChangeType change, string currentPath, string oldPath = null)
        {
            Change = change;
            OldPath = oldPath;
            CurrentPath = currentPath;
        }

        public FileChangeType Change { get; }

        public string OldPath { get; }

        public string CurrentPath { get; }
    }

    public interface IFolderWatcher : IDisposable
    {
        event EventHandler<FileChangedEventArgs> Changed;

        void BeginWatch(string path);

        void EndWatch();
    }

    public sealed class FolderWatcher : IFolderWatcher
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
        
        private volatile bool _isDisposing;
        private readonly Thread _monitoringThread;

        public FolderWatcher(IFileSystemProvider fileSystemProvider)
        {
            _fileSystemProvider = fileSystemProvider;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;

            _monitoringThread = new Thread(() =>
            {
                while (!_isDisposing)
                {
                    if (!_fileSystemWatcher.EnableRaisingEvents && !_isDisposing)
                        Thread.Sleep(100);

                    if (!_fileSystemProvider.IsDirectoryExists(_fileSystemWatcher.Path))
                        Changed?.Invoke(this, new FileChangedEventArgs(FileChangeType.Delete, _fileSystemWatcher.Path));

                    Thread.Sleep(100);
                }
            }){IsBackground = true};
            _monitoringThread.Start();
        }

        public event EventHandler<FileChangedEventArgs> Changed;

        public void BeginWatch(string path)
        {
            _fileSystemWatcher.Path = path;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void EndWatch()
        {
            _fileSystemWatcher.Created -= FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted -= FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed -= FileSystemWatcherOnRenamed;
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            Changed?.Invoke(this, new FileChangedEventArgs(FileChangeType.Create, e.FullPath));
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            Changed?.Invoke(this, new FileChangedEventArgs(FileChangeType.Delete, e.FullPath));
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            Changed?.Invoke(this, new FileChangedEventArgs(FileChangeType.Rename, e.FullPath, e.OldFullPath));
        }

        public void Dispose()
        {
            if (_isDisposing)
                return;

            _isDisposing = true;
            _fileSystemWatcher.EnableRaisingEvents = false;
            _fileSystemWatcher.Dispose();
        }
    }
}
