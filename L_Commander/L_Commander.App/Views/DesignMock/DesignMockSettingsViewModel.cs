using L_Commander.App.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockSettingsViewModel : SettingsViewModel
    {
        public DesignMockSettingsViewModel()
            : base(new DesignMockSettingsProvider())
        {
            Initialize();
        }
    }
}