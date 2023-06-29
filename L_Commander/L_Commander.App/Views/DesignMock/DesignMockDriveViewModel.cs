using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock
{
    internal class DesignMockDriveViewModel : DriveViewModel
    {
        public DesignMockDriveViewModel()
            : base(new DriveInfo("C:\\"))
        {

        }
    }
}
