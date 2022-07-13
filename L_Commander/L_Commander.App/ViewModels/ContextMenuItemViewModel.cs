using System.Windows.Media;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class ContextMenuItemViewModel : ViewModelBase
{
    public ImageSource Icon { get; set; }

    public string DisplayName { get; set; }

    public IDelegateCommand Command { get; set; }

    public bool IsCheckable { get; set; }

    public bool IsChecked { get; set; }

    public bool IsSeparator { get; set; }
}