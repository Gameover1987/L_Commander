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
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views
{
    /// <summary>
    /// Interaction logic for MultipleFilePropertiesWindow.xaml
    /// </summary>
    public partial class MultipleFilePropertiesWindow
    {
        private IMultipleFilePropertiesViewModel _multipleFilePropertiesViewModel;

        public MultipleFilePropertiesWindow()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _multipleFilePropertiesViewModel = (IMultipleFilePropertiesViewModel)DataContext;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _multipleFilePropertiesViewModel.Cancel();

            Close();
        }
    }
}
