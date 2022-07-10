using L_Commander.App.Infrastructure;
using L_Commander.App.ViewModels.Settings;
using System.Linq;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockTagSettingsItemViewModel : TagSettingsItemViewModel
    {
        public DesignMockTagSettingsItemViewModel()
            : base(CreateTagSettings(), new DesignMockAddTagViewModel(), new DesignMockWindowManager())
        {
            SelectedTag = Tags.FirstOrDefault();
        }

        private static TagSettings CreateTagSettings()
        {
            return new TagSettings
            {
                IsEnabled = true,
                Tags = new Tag[]
                {
                    new Tag()
                    {
                        Text = "Red",
                        Color = 16711680
                    },
                    new Tag()
                    {
                        Text = "Orange",
                        Color = 16753920
                    },
                    new Tag()
                    {
                        Text = "Green",
                        Color = 32768
                    },
                    new Tag()
                    {
                        Text = "Light blue",
                        Color = 11393254
                    },
                    new Tag()
                    {
                        Text = "Blue",
                        Color = 255
                    },
                    new Tag()
                    {
                        Text = "Violet",
                        Color = 15631086
                    },
                }
            };
        }
    }
}