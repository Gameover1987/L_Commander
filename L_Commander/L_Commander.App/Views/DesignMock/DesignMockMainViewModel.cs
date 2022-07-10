using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockSettingsProvider : ISettingsProvider
    {
        public void Save(ClientSettings settings)
        {

        }

        public ClientSettings Get()
        {
            return new ClientSettings
            {
                TagSettings = new TagSettings
                {
                    IsEnabled = true,
                    Tags = new Tag[]
                    {
                        new Tag
                        {
                            Text = "Important",
                            Color = 1,
                        },
                    }
                }
            };
        }
    }

    internal sealed class DesignMockMainViewModel : MainViewModel
    {
        public DesignMockMainViewModel()
            : base(new DesignMockSettingsProvider(),
                  new DesignMockFileManagerViewModel(),
                  new DesignMockFileManagerViewModel(),
                  new DesignMockCopyOperation(),
                  new DesignMockMoveOperation(),
                  new DesignMockDeleteOperation(),
                  new DesignMockWindowManager(),
                  new DesignMockExceptionHandler(),
                  new DesignMockSettingsViewModel())
        {
        }
    }
}
