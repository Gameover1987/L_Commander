using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.Common.Extensions
{
    public static class IntExtensions
    {
        public static string SizeAsString(this long totalSize)
        {
            if (totalSize < 1024)
                return $"{totalSize}";

            if (totalSize < 1024 * 1024)
            {
                return $"{Math.Round(totalSize / 1024.0, 2)} Kb";
            }

            if (totalSize < 1024 * 1024 * 1024)
            {
                return $"{Math.Round(totalSize / 1024.0 / 1024, 2)} Mb";
            }

            return $"{Math.Round(totalSize / 1024.0 / 1024 / 1024, 2)} Gb";
        }

        public static string ToStringSplitedBySpaces(this long totalSize)
        {
            if (totalSize == 0)
                return "0";

            var numberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = " " };
            var totalSizeStr = String.Format(numberFormatInfo, "{0:#,#}", totalSize);
            return totalSizeStr;
        }
    }
}
