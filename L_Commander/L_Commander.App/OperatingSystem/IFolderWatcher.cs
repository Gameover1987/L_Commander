using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem
{
    public enum FileChnageType
    {
        Create,
        Rename,
        Delete,
        Other
    }

    public class FileChangedEventArgs
    {
        public FileChangedEventArgs(FileChnageType change, string currentPath, string oldPath = null)
        {
            Change = change;
            OldPath = oldPath;
            CurrentPath = currentPath;
        }

        public FileChnageType Change { get; }

        public string OldPath { get; }

        public string CurrentPath { get; }
    }

    public interface IFolderWatcher
    {
        event EventHandler<FileChangedEventArgs> Changed;

        void BeginWatch(string path);

        void EndWatch(string path);
    }

    public sealed class FolderWatcher : IFolderWatcher
    {
        private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();

        public FolderWatcher()
        {
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;
        }

        public event EventHandler<FileChangedEventArgs> Changed;

        public void BeginWatch(string path)
        {
            _fileSystemWatcher.Path = path;
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void EndWatch(string path)
        {
            _fileSystemWatcher.Changed -= FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created -= FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted -= FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed -= FileSystemWatcherOnRenamed;
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            Changed?.Invoke(this, new FileChangedEventArgs(FileChnageType.Create, e.FullPath));
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            Changed?.Invoke(this, new FileChangedEventArgs(FileChnageType.Delete, e.FullPath));
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            Changed?.Invoke(this, new FileChangedEventArgs(FileChnageType.Rename, e.FullPath, e.OldFullPath));
        }
    }
}
