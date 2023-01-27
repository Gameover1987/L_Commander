using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace L_Commander.Common.Extensions
{
    public static class ColorExtensions
    {
        public static int ToInt(this Color color)
        {
            var key = color.R << 16 | color.G << 8 | color.B;
            return key;
        }

        public static Color ToColor(this int iCol)
        {
            var color = Color.FromRgb(
                (byte)(iCol >> 16),
                (byte)(iCol >> 8),
                (byte)iCol);
           
            return color;
        }
    }
}
