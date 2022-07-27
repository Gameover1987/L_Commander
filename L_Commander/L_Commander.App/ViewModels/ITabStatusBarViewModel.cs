using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels
{
    public interface ITabStatusBarViewModel
    {
        int TotalCount { get; }

        int SelectedCount { get; }

        string SelectionInfo { get; }

        void Update(IFileManagerTabViewModel tab);
    }

    public class TabStatusBarViewModel : ViewModelBase, ITabStatusBarViewModel
    {
        public void Update(IFileManagerTabViewModel tab)
        {
            TotalCount = tab.FileSystemEntries.Count;
            SelectedCount = tab.SelectedEntries.Length;

            var foldersCount = tab.SelectedEntries.Count(x => !x.IsFile);
            if (tab.SelectedEntries.Any(x => x.IsFile))
            {
                var filesCount = tab.SelectedEntries.Count(x => x.IsFile);
                var filesSize = GetFileSizeAsString(tab.SelectedEntries
                    .Where(x => x.IsFile)
                    .Select(x => x.TotalSize)
                    .Sum());

                SelectionInfo = $"{filesCount} files ({filesSize}), folders {foldersCount}";
            }
            else
            {
                SelectionInfo = $"0 files, folders {foldersCount}";
            }

            OnPropertyChanged();
        }

        public int TotalCount { get; private set; }

        public int SelectedCount { get; private set; }

        public string SelectionInfo { get; private set; }

        private string GetFileSizeAsString(long totalSize)
        {
            if (totalSize < 1024)
                return $"{totalSize}";

            if (totalSize < 1024 * 1024)
            {
                return $"~{totalSize / 1024} Kb";
            }

            if (totalSize < 1024 * 1024 * 1024)
            {
                return $"~{totalSize / 1024 / 1024} Mb";
            }

            return $"~{totalSize / 1024 / 1024 / 1024} Gb";
        }
    }
}
