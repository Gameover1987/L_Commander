using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace L_Commander.Common.Extensions
{
    public static class BitmapExtensions
    {
        public static ImageSource ToImageSource(string path)
        {
            using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(path))
            {
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                imageSource.Freeze();

                return imageSource;
            }
        }
    }
}
