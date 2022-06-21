using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.FileSystem
{
    public interface IDriveInfoProvider
    {
        DriveInfo[] GetDrives();
    }

    public sealed class DriveInfoProvider : IDriveInfoProvider
    {
        public DriveInfo[] GetDrives()
        {
            return DriveInfo.GetDrives();
        }
    }
}
