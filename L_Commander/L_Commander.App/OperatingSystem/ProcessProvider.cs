using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace L_Commander.App.OperatingSystem;

public sealed class ProcessProvider : IProcessProvider
{
    public void OpenFileWith(string filePath, ApplicationModel application)
    {
        var processStartInfo = new ProcessStartInfo(application.ExecutePath)
        {
            UseShellExecute = false,
            CreateNoWindow = false,
            Arguments = $"\"{filePath}\""
        };

        var process = new Process { StartInfo = processStartInfo };
        process.Start();
    }

    public void OpenFile(string path)
    {
        var processStartInfo = new ProcessStartInfo("explorer")
        {
            UseShellExecute = false,
            CreateNoWindow = false,
            Arguments = $"\"{path}\""
        };

        var process = new Process { StartInfo = processStartInfo };
        process.Start();
    }

    public void OpenExplorer(string path)
    {
        Process.Start("explorer.exe", path);
    }

    public void OpenTerminal(string path)
    {
        Process.Start(new ProcessStartInfo("cmd", $"/K \"cd /d {path}\""));
    }

    public void ShowPropertiesByPath(string path)
    {
     
    }
}