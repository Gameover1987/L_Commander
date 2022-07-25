using L_Commander.App.ViewModels.Settings;
using L_Commander.App.ViewModels.Factories;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockSettingsItemsViewModelFactory : SettingsItemsViewModelFactory
    {
        public DesignMockSettingsItemsViewModelFactory() 
            : base(new DesignMockWindowManager(), new DesignMockAddTagViewModel(), new DesignMockTagRepository())
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