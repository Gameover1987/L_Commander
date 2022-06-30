using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.Infrastructure
{
    public interface IWindowManager
    {
        Task<string> ShowInputBox(string title, string message, MetroDialogSettings settings = null);
    }

    public sealed class WindowManager : IWindowManager
    {
        public Task<string> ShowInputBox(string title, string message, MetroDialogSettings settings = null)
        {
            return DialogManager.ShowInputAsync((MetroWindow) Application.Current.MainWindow, title, message, settings);
        }
    }
}
