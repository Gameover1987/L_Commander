﻿using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using L_Commander.App.ViewModels;
using MahApps.Metro.Controls;

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
            var window = (MetroWindow)Window.GetWindow(this);
            var mainViewModel = (IMainViewModel) window.DataContext;
            if (mainViewModel.ActiveFileManager == mainViewModel.LeftFileManager)
            {
                var flyout = window.Flyouts.Items.Cast<Flyout>().First(x => (string) x.Tag == MainWindow.LeftFlyoutTag);
                flyout.IsOpen = !flyout.IsOpen;
            }else if (mainViewModel.ActiveFileManager == mainViewModel.RightFileManager)
            {
                var flyout = window.Flyouts.Items.Cast<Flyout>().First(x => (string) x.Tag == MainWindow.RightFlyoutTag);
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext == null)
                return;

            _dataContext.SelectedEntries = dataGrid.SelectedItems.Cast<IFileSystemEntryViewModel>().ToArray();            
        }
    }
}
