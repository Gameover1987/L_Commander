using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.Controls
{
    /// <summary>
    /// Interaction logic for FileManagerTabControl.xaml
    /// </summary>
    public partial class FileManagerTabControl : UserControl
    {
        public FileManagerTabControl()
        {
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            var dataGridRow = (DataGridRow) sender;
            var fileSystemEntryViewModel = (IFileSystemEntryViewModel) dataGridRow.DataContext;
            fileSystemEntryViewModel.Initialize();

            Debug.WriteLine(fileSystemEntryViewModel.Path);
        }
    }
}
