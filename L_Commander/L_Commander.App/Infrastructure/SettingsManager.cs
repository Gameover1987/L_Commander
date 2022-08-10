using System;
using System.IO;
using System.Linq;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.App.OperatingSystem;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace L_Commander.App.Infrastructure;

public sealed class SettingsManager : ISettingsManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IFileSystemProvider _fileSystemProvider;

    private const string ClientSettingsFileName = "ClientSettings.json";

    public SettingsManager(IServiceProvider serviceProvider, IFileSystemProvider fileSystemProvider)
    {
        _serviceProvider = serviceProvider;
        _fileSystemProvider = fileSystemProvider;
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
        if (!_fileSystemProvider.IsFileExists(ClientSettingsFileName))
            return GetDefaultSettings();

        return JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText(ClientSettingsFileName));
    }

    public event EventHandler<SettingsChangedEventArgs> SettingsChanged;

    private ClientSettings GetDefaultSettings()
    {
        return new ClientSettings()
        {
            FilesAndFoldersSettings = new FilesAndFoldersSettings
            {
                ShowHiddenFilesAndFolders = true,
                ShowSystemFilesAndFolders = true
            }
        };
    }
}