using L_Commander.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.ViewModels.Settings
{
    public class TagSettingsItemViewModel : ViewModelBase, ISettingsItemViewModel
    {
        public string DisplayName { get { return "Tag settings"; } }
    }
}
