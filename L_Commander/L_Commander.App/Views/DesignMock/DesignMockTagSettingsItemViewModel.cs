using L_Commander.App.Infrastructure;
using L_Commander.App.ViewModels.Settings;
using System.Linq;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockTagSettingsItemViewModel : TagSettingsItemViewModel
    {
        public DesignMockTagSettingsItemViewModel()
            : base(new DesignMockTagRepository(), new DesignMockAddTagViewModel(), new DesignMockWindowManager())
        {
            SelectedTag = Tags.FirstOrDefault();
        }
    }
}