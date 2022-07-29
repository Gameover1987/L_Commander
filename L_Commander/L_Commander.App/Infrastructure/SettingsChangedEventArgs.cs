using L_Commander.App.Infrastructure.Settings;
using System;

namespace L_Commander.App.Infrastructure;

public class SettingsChangedEventArgs : EventArgs
{
    public SettingsChangedEventArgs(ClientSettings settings)
    {
        Settings = settings;
    }

    public ClientSettings Settings { get; set; }
}