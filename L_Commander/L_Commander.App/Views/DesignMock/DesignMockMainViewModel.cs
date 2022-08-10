using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Factories;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockSettingsManager : ISettingsManager
    {
        public void Save()
        {
            
        }

        public ClientSettings Get()
        {
            return new ClientSettings
            {
                
            };
        }

        public event EventHandler<SettingsChangedEventArgs> SettingsChanged;
    }

    internal sealed class DesignMockMainViewModel : MainViewModel
    {
        public DesignMockMainViewModel()
            : base(new DesignMockSettingsManager(),
                  new DesignMockFileManagerViewModel(),
                  new DesignMockFileManagerViewModel(),
                  new DesignMockCopyOperation(),
                  new DesignMockMoveOperation(),
                  new DesignMockDeleteOperation(),
                  new DesignMockWindowManager(),
                  new DesignMockHistoryViewModel(),
                  new DesignMockExceptionHandler(),
                  new DesignMockSettingsViewModel(),
                  new FileSystemProvider(new IconCache()),
                  new FileSystemEntryViewModelFactory(new FileSystemProvider(new IconCache()), new DesignMockExceptionHandler(), new DesignMockTagRepository()))
        {
        }
    }
}
