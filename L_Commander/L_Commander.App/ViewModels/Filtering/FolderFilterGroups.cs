using System.Collections.Generic;

namespace L_Commander.App.ViewModels.Filtering;

public static class FolderFilterGroups
{
    private static readonly Dictionary<string, string> _filterGroupDictionary = new Dictionary<string, string>();

    static FolderFilterGroups()
    {
        _filterGroupDictionary.Add(".rar", "Archives");
        _filterGroupDictionary.Add(".zip", "Archives");

        _filterGroupDictionary.Add(".mp3", "Audio");
        _filterGroupDictionary.Add(".wma", "Audio");
        _filterGroupDictionary.Add(".wav", "Audio");

        _filterGroupDictionary.Add(".config", "Config");
        _filterGroupDictionary.Add(".ini", "Config");

        _filterGroupDictionary.Add(".bat", "Executables");
        _filterGroupDictionary.Add(".exe", "Executables");
        _filterGroupDictionary.Add(".com", "Executables");
        _filterGroupDictionary.Add(".msi", "Executables");

        _filterGroupDictionary.Add(".doc", "Documents");
        _filterGroupDictionary.Add(".docx", "Documents");
        _filterGroupDictionary.Add(".xlsx", "Documents");
        _filterGroupDictionary.Add(".pptx", "Documents");
        _filterGroupDictionary.Add(".pdf", "Documents");
        _filterGroupDictionary.Add(".rtf", "Documents");

        _filterGroupDictionary.Add(".gp", "Guitar Pro");
        _filterGroupDictionary.Add(".gp3", "Guitar Pro");
        _filterGroupDictionary.Add(".gp4", "Guitar Pro");
        _filterGroupDictionary.Add(".gp5", "Guitar Pro");

        _filterGroupDictionary.Add(".bmp", "Images");
        _filterGroupDictionary.Add(".jpeg", "Images");
        _filterGroupDictionary.Add(".jpg", "Images");
        _filterGroupDictionary.Add(".svg", "Images");
        _filterGroupDictionary.Add(".psd", "Images");
        _filterGroupDictionary.Add(".png", "Images");

        _filterGroupDictionary.Add(".avi", "Video");
        _filterGroupDictionary.Add(".mkv", "Video");
        _filterGroupDictionary.Add(".mov", "Video");
        _filterGroupDictionary.Add(".mp4", "Video");
        _filterGroupDictionary.Add(".mpg", "Video");

        _filterGroupDictionary.Add(".epub", "Text");
        _filterGroupDictionary.Add(".fb2", "Text");
    }

    public static Dictionary<string, string> Items => _filterGroupDictionary;
}