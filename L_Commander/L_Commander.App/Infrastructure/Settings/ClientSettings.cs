namespace L_Commander.App.Infrastructure.Settings;

/// <summary>
/// Application settings
/// </summary>
public class ClientSettings
{
    public ClientSettings()
    {
        FilesAndFoldersAppearanceSettings = new FilesAndFoldersAppearanceSettings();
    }

    public MainWindowSettings MainWindowSettings { get; set; }

    public FileManagerSettings LeftFileManagerSettings { get; set; }

    public FileManagerSettings RightFileManagerSettings { get; set; }

    public FilesAndFoldersAppearanceSettings FilesAndFoldersAppearanceSettings { get; set; }
}