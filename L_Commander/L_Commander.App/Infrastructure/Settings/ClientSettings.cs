namespace L_Commander.App.Infrastructure.Settings;

/// <summary>
/// Application settings
/// </summary>
public class ClientSettings
{
    public ClientSettings()
    {
        FilesAndFoldersSettings = new FilesAndFoldersSettings();
    }

    public MainWindowSettings MainWindowSettings { get; set; }

    public FileManagerSettings LeftFileManagerSettings { get; set; }

    public FileManagerSettings RightFileManagerSettings { get; set; }

    public FilesAndFoldersSettings FilesAndFoldersSettings { get; set; }

    public FavoritesSettings FavoritesSettings { get; set; }
}