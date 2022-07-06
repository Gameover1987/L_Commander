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
            throw new System.NotImplementedException();
        }

        public ClientSettings Get()
        {
            throw new System.NotImplementedException();
        }
    }

    internal sealed class DesignMockMainViewModel : MainViewModel
    {
        public DesignMockMainViewModel() 
            : base(new DesignMockSettingsProvider(),new DesignMockFileManagerViewModel(), new DesignMockFileManagerViewModel(), new DesignMockCopyOperation(), new DesignMockWindowManager(), new DesignMockExceptionHandler())
        {
        }
    }
}
