using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.OperatingSystem.Operations;
using L_Commander.App.ViewModels;
using L_Commander.Common.Extensions;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace L_Commander.App.Views.Controls
{
    public interface IFileManagerTabControl
    {
        void FocusToSelection();
    }

    /// <summary>
    /// Interaction logic for FileManagerTabControl.xaml
    /// </summary>
    public partial class FileManagerTabControl : UserControl, IFileManagerTabControl
    {
        private IFileManagerTabViewModel _fileManagerTab;

        private readonly Thickness _oldThickness;
        private readonly Brush _oldBorderBrush;

        public FileManagerTabControl()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;

            _oldThickness = dataGrid.BorderThickness;
            _oldBorderBrush = dataGrid.BorderBrush;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _fileManagerTab = (IFileManagerTabViewModel)e.NewValue;
            _fileManagerTab.Attach(this);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _fileManagerTab.OpenCommand.TryExecute();
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
            if (_fileManagerTab == null)
                return;

            _fileManagerTab.SelectedEntries = dataGrid.SelectedItems.Cast<IFileSystemEntryViewModel>().ToArray();
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

        private void DataGrid_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                dataGrid.BorderThickness = new Thickness(1);
                dataGrid.BorderBrush = Brushes.Black;
            }
        }

        private void DataGrid_OnDragLeave(object sender, DragEventArgs e)
        {
            dataGrid.BorderBrush = _oldBorderBrush;
            dataGrid.BorderThickness = _oldThickness;
        }

        private void DataGrid_OnDrop(object sender, DragEventArgs e)
        {
            try
            {
                var move = e.KeyStates == DragDropKeyStates.ShiftKey;

                var descriptors = Array.Empty<FileSystemEntryDescriptor>();

                var draggedObjects = (object[])e.Data.GetData(typeof(object[]));
                if (draggedObjects != null)
                {
                    descriptors = draggedObjects
                        .Cast<IFileSystemEntryViewModel>()
                        .Select(x => x.GetDescriptor())
                        .ToArray();
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files == null)
                        return;
                    var fileSystemProvider = App.ServiceProvider.GetService<IFileSystemProvider>();

                    descriptors = files.Select(x => fileSystemProvider.GetFileSystemDescriptor(x)).ToArray();
                }

                if (descriptors.Length == 0) 
                    return;

                var mainViewModel = App.ServiceProvider.GetService<IMainViewModel>();
                if (mainViewModel == null)
                    throw new ArgumentException($"{nameof(IMainViewModel)} is not registered in DI");

                if (move)
                {
                    mainViewModel.Move(descriptors, _fileManagerTab.FullPath);
                }
                else
                {
                    mainViewModel.Copy(descriptors, _fileManagerTab.FullPath);
                }
            }
            catch (Exception exception)
            {
                var exceptionHandler = App.ServiceProvider.GetService<IExceptionHandler>();
                exceptionHandler?.HandleException(exception);
            }
            finally
            {
                dataGrid.BorderBrush = _oldBorderBrush;
                dataGrid.BorderThickness = _oldThickness;
            }
        }

        private void DataGrid_OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
                e.Action = DragAction.Cancel;
        }

        public void FocusToSelection()
        {
            ThreadTaskExtensions.Run(() =>
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    var selectedRow = dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem);
                    if (selectedRow == null)
                    {
                        var aaa = dataGrid.ItemContainerGenerator.Status;
                        return;
                    }

                    var firstCell = selectedRow.FindChild<DataGridCell>();
                    firstCell.Focus();
                });
            });

        }
    }
}
