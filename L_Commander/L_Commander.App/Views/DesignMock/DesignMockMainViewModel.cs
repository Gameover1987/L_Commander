using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.ViewModels;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockMainViewModel : MainViewModel
    {
        public DesignMockMainViewModel() 
            : base(new DesignMockFileManagerViewModel(), new DesignMockFileManagerViewModel())
        {
        }
    }
}
