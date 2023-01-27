using System;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Infrastructure
{
    public interface ISettingsManager
    {
        void Save();

        ClientSettings Get();

        event EventHandler<SettingsChangedEventArgs> SettingsChanged;
    }
}
