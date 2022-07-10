using L_Commander.App.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.ViewModels.Factories;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockSettingsItemsViewModelFactory : SettingsItemsViewModelFactory
    {
        public DesignMockSettingsItemsViewModelFactory() 
            : base(new DesignMockWindowManager(), new DesignMockAddTagViewModel())
        {
        }
    }

    internal sealed class DesignMockSettingsViewModel : SettingsViewModel
    {
        public DesignMockSettingsViewModel()
            : base(new DesignMockSettingsProvider(), new DesignMockSettingsItemsViewModelFactory() )
        {
            Initialize();
        }
    }
}