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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.Views.Controls
{
    /// <summary>
    /// Interaction logic for OpenWithDialog.xaml
    /// </summary>
    public partial class OpenWithDialog : BaseMetroDialog
    {
        public OpenWithDialog()
            : this(null, null)
        {
            InitializeComponent();
        }

        public OpenWithDialog(MetroWindow? parentWindow, MetroDialogSettings? settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
        }

        private async void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close(sender);
        }

        private async void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close(sender);
        }

        private void Grid_mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Close(sender);
            }
        }

        private async void Close(object sender)
        {
            var dialog = ((DependencyObject)sender).TryFindParent<BaseMetroDialog>()!;
            var window = (MetroWindow)Window.GetWindow(dialog);
            await window.HideMetroDialogAsync(dialog);
        }
    }
}
