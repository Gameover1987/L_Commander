using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using L_Commander.App.ViewModels.Settings;
using L_Commander.Common.Extensions;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockAddTagViewModel : AddTagViewModel
    {
        public DesignMockAddTagViewModel()
        {
            Text = "Cats!";
            Color = Colors.LightGray;
        }
    }
}
