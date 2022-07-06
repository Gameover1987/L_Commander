using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace L_Commander.App.Views.Controls
{
    public class CustomDataGrid : DataGrid
    {
        static CustomDataGrid()
        {
            Type ownerType = typeof(CustomDataGrid);

            ItemsSourceProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((PropertyChangedCallback)null));
        }
    }
}
