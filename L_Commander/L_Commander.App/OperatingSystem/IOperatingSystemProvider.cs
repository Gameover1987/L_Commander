using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem
{
    public interface IOperatingSystemProvider
    {
        void OpenExplorer(string path);

        void OpenTerminal(string path);
    }

    public sealed class OperatingSystemProvider : IOperatingSystemProvider
    {
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
