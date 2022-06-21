using System.Windows;
using System.Windows.Input;
using L_Commander.UI.ViewModels;

namespace L_Commander.UI.Windows
{
    /// <summary>
    /// Interaction logic for ErrorMessageWindow.xaml
    /// </summary>
    public partial class ErrorMessageWindow : Window
	{
		public ErrorMessageWindow()
		{
			InitializeComponent();

			KeyDown += OnMessageWindowKeyDown;
		}

		private void OnMessageWindowKeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Key == Key.C) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				OnCopyClick(this, null);
			}
		}

		private void OnCopyClick(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as ErrorMessageViewModel;
			if (vm != null)
			{
				Clipboard.SetText(vm.GetFullInfoText());
			}
		}
	}
}
