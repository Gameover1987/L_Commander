namespace L_Commander.App.Infrastructure;

public class MainWindowSettings
{
    public double Left { get; set; }

    public double Top { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }
}

public class TabSettings
{
    public string Path { get; set; }

    public bool IsLocked { get; set; }
}

public class FileManagerSettings
{
    public TabSettings[] Tabs { get; set; }

    public string SelectedPath { get; set; }

}

public class ClientSettings
{
    public MainWindowSettings MainWindowSettings { get; set; }

    public FileManagerSettings LeftFileManagerSettings { get; set; }

    public FileManagerSettings RightFileManagerSettings { get; set; }
}