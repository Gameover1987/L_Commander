using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IWindow
    {
        private IMainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            Loaded+= OnLoaded;
            Closing += OnClosing;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _mainViewModel = (IMainViewModel)DataContext;
            _mainViewModel?.Initialize(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var mainWindowSettings = _mainViewModel.GetMainWindowSettings();
            if (mainWindowSettings == null)
                return;

            Left = mainWindowSettings.Left;
            Top = mainWindowSettings.Top;
            Width = mainWindowSettings.Width;
            Height = mainWindowSettings.Height;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _mainViewModel.SaveSettings();
        }

        private void LeftFileManager_OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _mainViewModel.ActiveFileManager = _mainViewModel.LeftFileManager;
        }

        private void LeftFileManager_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainViewModel.ActiveFileManager = _mainViewModel.LeftFileManager;
        }

        private void RightFileManager_OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _mainViewModel.ActiveFileManager = _mainViewModel.RightFileManager;
        }

        private void RightFileManager_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainViewModel.ActiveFileManager = _mainViewModel.RightFileManager;
        }
    }
}