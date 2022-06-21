using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace L_Commander.UI.Helpers
{
    public static class WindowCloseHelper
    {
        public static readonly DependencyProperty CloseKeyProperty = DependencyProperty.RegisterAttached(
            "CloseKey", typeof(Key), typeof(WindowHelper), new PropertyMetadata(default(Key), ClosePropertyChangedCallback));

        private static void ClosePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(sender))
                return;

            var window = (Window)sender;
            window.PreviewKeyDown += WindowOnPreviewKeyDown;
        }

        private static void WindowOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var window = (Window)sender;
            var closeKey = GetCloseKey(window);
            if (e.Key == closeKey)
            {
                window.PreviewKeyDown -= WindowOnPreviewKeyDown;
                window.Close();

                e.Handled = true;
            }
        }

        public static void SetCloseKey(DependencyObject element, Key value)
        {
            element.SetValue(CloseKeyProperty, value);
        }

        public static Key GetCloseKey(DependencyObject element)
        {
            return (Key)element.GetValue(CloseKeyProperty);
        }

        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached(
            "DialogResult", typeof(bool?), typeof(WindowCloseHelper), new PropertyMetadata(default(bool?), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            button.Click += ButtonOnClick;
        }

        private static void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Click -= ButtonOnClick;
            var window = Window.GetWindow(button);
            var dialogResult = WindowCloseHelper.GetDialogResult(button);
            window.DialogResult = dialogResult;
        }

        public static void SetDialogResult(DependencyObject element, bool? value)
        {
            element.SetValue(DialogResultProperty, value);
        }

        public static bool? GetDialogResult(DependencyObject element)
        {
            return (bool?)element.GetValue(DialogResultProperty);
        }
    }
}
