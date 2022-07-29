using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using L_Commander.App.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;

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
            _dataContext = (IFileManagerTabViewModel)e.NewValue;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _dataContext.OpenCommand.TryExecute();
        }

        private void FilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowFiltersPanel();
        }

        private void ShowFiltersPanel()
        {
            var window = (MetroWindow)Window.GetWindow(this);
            var mainViewModel = (IMainViewModel)window.DataContext;
            if (mainViewModel.ActiveFileManager == mainViewModel.LeftFileManager)
            {
                var flyout = window.Flyouts.Items.Cast<Flyout>().First(x => (string)x.Tag == MainWindow.LeftFlyoutTag);
                flyout.IsOpen = !flyout.IsOpen;
            }
            else if (mainViewModel.ActiveFileManager == mainViewModel.RightFileManager)
            {
                var flyout = window.Flyouts.Items.Cast<Flyout>().First(x => (string)x.Tag == MainWindow.RightFlyoutTag);
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext == null)
                return;

            _dataContext.SelectedEntries = dataGrid.SelectedItems.Cast<IFileSystemEntryViewModel>().ToArray();
        }

        private void DataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var window = (MetroWindow)Window.GetWindow(this);
                var mainViewModel = (IMainViewModel)window.DataContext;
                mainViewModel.DeleteCommand.TryExecute();
            }
        }

        private void DataGridRow_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var dataGridRow = (DataGridRow)sender;
            var fileSystemEntryViewModel = (IFileSystemEntryViewModel)dataGridRow.DataContext;

            var contextMenuItemProvider = App.ServiceProvider.GetService<IContextMenuItemProvider>();

            if (dataGridRow.ContextMenu != null)
                dataGridRow.ContextMenu.ItemsSource = contextMenuItemProvider.GetMenuItems(fileSystemEntryViewModel); ;
        }

        private void DataGridRow_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            tagsColumn.Width = 0;
            tagsColumn.Width = DataGridLength.Auto;
        }

        private void PathTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var bindingExpression = pathTextBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.UpdateTarget();

            pathPartItemsControl.Visibility = Visibility.Visible;
        }

        private void PathTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            var bindingExpression = pathTextBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.UpdateSource();
        }

        private void PathPartsItemControl_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pathPartItemsControl.Visibility = Visibility.Collapsed;
            pathTextBox.Focus();
        }

        private void FileManagerTabControl_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ShowFiltersPanel();
            }
        }

        private void DataGridRow_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var dataGridRow = (DataGridRow)sender;
            var firstCell = dataGridRow.FindChild<DataGridCell>();
            firstCell.Focus();
        }

        private void DataGrid_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var dataGrid = e.Source as DataGrid;
            if (dataGrid == null)
                return;

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed &&
                _dataContext.SelectedEntries.Any())
                DragDrop.DoDragDrop(dataGrid, new DragDropData(dataGrid, _dataContext.SelectedEntries), DragDropEffects.Copy);
        }

        private void DataGrid_OnDrop(object sender, DragEventArgs e)
        {
            var data = (DragDropData)e.Data.GetData(typeof(DragDropData));
            if (data.DataGrid == dataGrid)
                return;

            var window = Window.GetWindow(this);
            var mainViewModel = window.DataContext as IMainViewModel;
            mainViewModel.CopyCommand.TryExecute();
        }

        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            //var dataGridRow = sender as DataGridRow;
            //if (e.ClickCount == 2)
            //    return;

            //if (dataGridRow.IsSelected && _dataContext.SelectedEntries.Length > 1)
            //    e.Handled = true;
        }

        private class DragDropData
        {
            public DragDropData(DataGrid dataGrid, IFileSystemEntryViewModel[] selectedItems)
            {
                DataGrid = dataGrid;
                SelectedItems = selectedItems;
            }

            public DataGrid DataGrid { get; }

            public IFileSystemEntryViewModel[] SelectedItems { get; }
        }

        private void ButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dataGridRow = sender as DataGridRow;
            if (dataGridRow == null)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Debug.WriteLine("Keyboard CTRL");
                if (dataGridRow.IsSelected)
                {
                    dataGrid.SelectedItems.Remove(dataGridRow.DataContext);
                }
                else
                {
                    dataGrid.SelectedItems.Add(dataGridRow.DataContext);
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                Debug.WriteLine("Keyboard SHIFT");
                var lastSelectedItem = dataGrid.SelectedItems[dataGrid.SelectedItems.Count - 1];
                foreach (var dataGridItem in dataGrid.Items)
                {
                    Debug.WriteLine(((IFileSystemEntryViewModel)dataGridItem).Name);
                }
                var currentSelectedItem = dataGridRow.DataContext;
            }
            else
            {
                Debug.WriteLine("Single click");
                dataGrid.SelectedItems.Clear();
                dataGridRow.IsSelected = !dataGridRow.IsSelected;
            }
        }
    }
}
