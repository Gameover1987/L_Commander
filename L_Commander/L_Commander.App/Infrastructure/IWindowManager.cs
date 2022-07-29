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
        MetroWindow MainWindow { get; }

        Task<string> ShowInputBox(string title, string message, MetroDialogSettings settings = null);

        Task<MessageDialogResult> ShowMessage(string title, string message, MetroDialogSettings settings = null);

        Task<MessageDialogResult> ShowQuestion(string title, string message, MetroDialogSettings settings = null);

        Task<ProgressDialogController> ShowProgressDialog(string title, string message);

        Task ShowDialogAsync<T>(object dataContext) where T : BaseMetroDialog, new();

        bool ShowDialogWindow<T>(object dataContext) where T : Window;

        void ShowWindow<T>(object dataContext) where T : Window;
    }
}
