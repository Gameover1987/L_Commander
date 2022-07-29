using L_Commander.App.ViewModels;
using L_Commander.UI.Helpers;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace L_Commander.App.Views.Controls
{
    /// <summary>
    /// Interaction logic for FileManagerControl.xaml
    /// </summary>
    public partial class FileManagerControl : UserControl
    {
        private IFileManagerViewModel _fileManager;

        public FileManagerControl()
        {
            InitializeComponent();

            DataContextChanged += FileManagerControl_DataContextChanged;
        }

        public void FocusTabControl()
        {
            var dataGrid = UIHelper.FindChild<DataGrid>(metroTabControl);
            if (dataGrid.SelectedItem != null)
            {
                var selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem);
                if (selectedRow == null)
                {
                    dataGrid.Focus();
                }
                else
                {
                    FocusManager.SetIsFocusScope(selectedRow, true);
                    FocusManager.SetFocusedElement(selectedRow, selectedRow);
                }
            }
            else
            {
                dataGrid.Focus();
            }
        }

        private void FileManagerControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IFileManagerViewModel)
                _fileManager = (IFileManagerViewModel)e.NewValue;
        }

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var tabItem = e.Source as TabItem;

            if (tabItem == null)
                return;

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.Move);
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            var sourceTabItem = (MetroTabItem)e.Data.GetData(typeof(MetroTabItem));
            var targetTabItem = UIHelper.FindAncestor<TabItem>((DependencyObject)e.OriginalSource);

            if (sourceTabItem == null || targetTabItem == null)
                return;

            var sourceFileManagerTab = (IFileManagerTabViewModel)sourceTabItem.DataContext;
            var targetFileManagerTab = (IFileManagerTabViewModel)targetTabItem.DataContext;

            _fileManager.SwapTabs(sourceFileManagerTab, targetFileManagerTab);
        }
    }
}
