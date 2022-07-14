using System.Collections.Generic;

namespace L_Commander.App.Infrastructure;

/// <summary>
/// Main window settings
/// </summary>
public class MainWindowSettings
{
    public double Left { get; set; }

    public double Top { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }
}

/// <summary>
/// File manager tab settings
/// </summary>
public class TabSettings
{
    public string Path { get; set; }

    public bool IsLocked { get; set; }
}

/// <summary>
/// File manager settings
/// </summary>
public class FileManagerSettings
{
    public TabSettings[] Tabs { get; set; }

    public string SelectedPath { get; set; }

}

/// <summary>
/// Tag settings
/// </summary>
public class TagSettings
{
    public bool IsEnabled { get; set; }

    public Tag[] Tags { get; set; }
}

/// <summary>
/// Application settings
/// </summary>
public class ClientSettings
{
    public MainWindowSettings MainWindowSettings { get; set; }

    public FileManagerSettings LeftFileManagerSettings { get; set; }

    public FileManagerSettings RightFileManagerSettings { get; set; }

    public TagSettings TagSettings { get; set; }
}