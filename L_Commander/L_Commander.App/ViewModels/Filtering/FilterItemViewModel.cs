﻿using System;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels.Filtering;

public class FilterItemViewModel : ViewModelBase
{
    private bool _isChecked;

    public FilterItemViewModel(string name)
    {
        Name = name;
        _isChecked = true;
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value)
                return;
            _isChecked = value;
            OnPropertyChanged(() => IsChecked);

            Checked?.Invoke(this, EventArgs.Empty);
        }
    }

    public string Name { get; set; }
    
    public event EventHandler Checked;

    public void SetIsChecked(bool value)
    {
        _isChecked = value;
        OnPropertyChanged(() => IsChecked);
    }
}