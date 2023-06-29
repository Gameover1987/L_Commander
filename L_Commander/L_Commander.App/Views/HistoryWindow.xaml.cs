using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using L_Commander.App.Infrastructure.History;
using L_Commander.App.ViewModels.History;

namespace L_Commander.App.Views
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow
    {
        private IHistoryViewModel _history;

        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HistoryWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _history = (IHistoryViewModel)DataContext;
        }

        private void HistoryListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_history == null)
                return;

            _history.SelectedHistoryItems = historyListBox.SelectedItems.Cast<HistoryItem>().ToArray();
        }
    }
}
