using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem
{
    public interface IOperatingSystemProvider
    {
        void OpenFile(string path);

        void OpenExplorer(string path);

        void OpenTerminal(string path);
    }

    public sealed class OperatingSystemProvider : IOperatingSystemProvider
    {
        public void OpenFile(string path)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(path);
            processStartInfo.Arguments = Path.GetFileName(path);
            processStartInfo.UseShellExecute = true;
            processStartInfo.FileName = path;
            processStartInfo.Verb = "OPEN";
            Process.Start(processStartInfo);
        }

        public void OpenExplorer(string path)
        {
            Process.Start("explorer.exe", path);
        }

        public void OpenTerminal(string path)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/K \"cd /d {path}\""));
        }
    }
}
