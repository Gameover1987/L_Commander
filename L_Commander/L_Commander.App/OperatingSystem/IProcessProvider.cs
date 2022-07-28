namespace L_Commander.App.OperatingSystem
{
    public interface IProcessProvider
    {
        void OpenFileWith(string filePath, ApplicationModel application);

        void OpenFile(string path);

        void OpenExplorer(string path);

        void OpenTerminal(string path);

        void ShowPropertiesByPath(string path);
    }
}
