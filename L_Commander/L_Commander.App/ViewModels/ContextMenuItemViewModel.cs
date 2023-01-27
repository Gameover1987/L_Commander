using System.Collections.ObjectModel;
using System.Windows.Media;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class ContextMenuItemViewModel : ViewModelBase
{
    private bool _isChecked;

    public ContextMenuItemViewModel()
    {
        IsEnabled = true;
    }

    public IDelegateCommand Command { get; set; }

    public string GestureText { get; set; }

    public bool IsDefault { get; set; }

    public bool IsCheckable { get; set; }

    public bool IsChecked
    {
        get { return _isChecked; }
        set
        {
            if (_isChecked == value)
                return;
            _isChecked = value;
            OnPropertyChanged(() => IsChecked);
        }
    }

    public bool IsSeparator { get; protected set; }

    public bool IsEnabled { get; set; }

    public object Header { get; set; }

    public ObservableCollection<ContextMenuItemViewModel> Children { get; } = new ObservableCollection<ContextMenuItemViewModel>();
}

public class SeparatorContextMenuItemViewModel : ContextMenuItemViewModel
{
    public SeparatorContextMenuItemViewModel()
    {
        IsSeparator = true;
    }
}