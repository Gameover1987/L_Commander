using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.OperatingSystem;
using L_Commander.App.OperatingSystem.Registry;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockOpenWithViewModel : OpenWithViewModel
    {
        public DesignMockOpenWithViewModel() 
            : base(new FileSystemProvider(new IconCache()), new ApplicationsProvider(new RegistryProvider()))
        {
            Initialize("123.jpg");
            SelectedApp = Apps.FirstOrDefault();
        }
    }
}
