using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.Infrastructure;

public sealed class WindowManager : IWindowManager
{
    public MetroWindow MainWindow => (MetroWindow)Application.Current.MainWindow;

    public MetroWindow ActiveWindow
    {
        get
        {
           return (MetroWindow)Application.Current.Windows.Cast<object>().Where(x => x is MetroWindow).FirstOrDefault(x => ((MetroWindow)x).IsActive);
        }
    }

    public Task<string> ShowInputBox(string title, string message, MetroDialogSettings settings = null)
    {
        return ActiveWindow.ShowInputAsync(title, message, settings);
    }

    public Task<MessageDialogResult> ShowMessage(string title, string message, MetroDialogSettings settings = null)
    {
        return ActiveWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, settings);
    }

    public Task<MessageDialogResult> ShowQuestion(string title, string message, MetroDialogSettings settings = null)
    {
        var window = ActiveWindow ?? MainWindow;
        return window.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, settings);
    }

    public Task<ProgressDialogController> ShowProgressDialog(string title, string message)
    {
        return ActiveWindow.ShowProgressAsync(title, message, isCancelable: true);
    }

    public async Task ShowDialogAsync<T>(object dataContext) where T : BaseMetroDialog, new()
    {
        var dialog = await DialogManager.ShowMetroDialogAsync<T>(MainWindow);
        dialog.DataContext = dataContext;
        await dialog.WaitUntilUnloadedAsync();
    }

    public bool ShowDialogWindow<T>(object dataContext) where T : Window
    {
        var window = Activator.CreateInstance<T>();
        window.Owner = MainWindow;
        window.DataContext = dataContext;
        return window.ShowDialog() == true;
    }

    public void ShowWindow<T>(object dataContext) where T : Window
    {
        var dialog = Activator.CreateInstance<T>();
        dialog.Owner = MainWindow;
        dialog.DataContext = dataContext;

        dialog.Show();
    }
}