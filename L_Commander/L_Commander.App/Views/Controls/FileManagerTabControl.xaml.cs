using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.Controls
{
    /// <summary>
    /// Interaction logic for FileManagerTabControl.xaml
    /// </summary>
    public partial class FileManagerTabControl : UserControl
    {
        private IFileManagerTabViewModel _dataContext;

        public FileManagerTabControl()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _dataContext = (IFileManagerTabViewModel) e.NewValue;
        }

        private void DataGridRow_Loaded(object sender, RoutedEventArgs e)
        {
            //var dataGridRow = (DataGridRow) sender;
            //var fileSystemEntryViewModel = (IFileSystemEntryViewModel) dataGridRow.DataContext;
            //fileSystemEntryViewModel.Initialize();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _dataContext.OpenCommand.TryExecute();
        }
    }
}
