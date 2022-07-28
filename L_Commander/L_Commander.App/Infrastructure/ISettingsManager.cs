using System;
using System.IO;
using L_Commander.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace L_Commander.App.Infrastructure
{
    public interface ISettingsFiller
    {
        void FillSettings(ClientSettings settings);
    }

    public class SettingsChangedEventArgs : EventArgs
    {
        public SettingsChangedEventArgs(ClientSettings settings)
        {
            Settings = settings;
        }

        public ClientSettings Settings { get; set; }
    }

    public interface ISettingsManager
    {
        void Save();

        ClientSettings Get();

        event EventHandler<SettingsChangedEventArgs> SettingsChanged;
    }

    public sealed class SettingsManager : ISettingsManager
    {
        private readonly IServiceProvider _serviceProvider;
        
        private const string ClientSettingsFileName = "ClientSettings.json";

        public SettingsManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Save()
        {
            var settings = new ClientSettings();

            var settingsFillers = _serviceProvider.GetServices<ISettingsFiller>();
            foreach (var settingsFiller in settingsFillers)
            {
                settingsFiller.FillSettings(settings);
            }

            var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(ClientSettingsFileName, settingsJson);

            SettingsChanged?.Invoke(this, new SettingsChangedEventArgs(settings));
        }

        public ClientSettings Get()
        {
            if (!File.Exists(ClientSettingsFileName))
                return null;

            return JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText(ClientSettingsFileName));
        }

        public event EventHandler<SettingsChangedEventArgs> SettingsChanged;
    }
}
