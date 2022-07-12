using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using L_Commander.App.ViewModels;
using MahApps.Metro.Controls;

namespace L_Commander.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IWindow
    {
        private IMainViewModel _mainViewModel;

        public const string LeftFlyoutTag = "Left";
        public const string RightFlyoutTag = "Right";

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _mainViewModel = (IMainViewModel)DataContext;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Initialize();

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