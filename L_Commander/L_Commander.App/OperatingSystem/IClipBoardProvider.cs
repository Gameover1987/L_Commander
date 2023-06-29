using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace L_Commander.App.OperatingSystem
{
    public interface IClipBoardProvider
    {
        void CopyToClipBoard(string data);
    }

    public sealed class ClipBoardProvider : IClipBoardProvider
    {
        public void CopyToClipBoard(string data)
        {
            Clipboard.SetText(data);
        }
    }
}
