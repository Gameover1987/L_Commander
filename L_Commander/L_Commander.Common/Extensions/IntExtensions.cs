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
                return $"{totalSize / 1024} Kb";
            }

            if (totalSize < 1024 * 1024 * 1024)
            {
                return $"{totalSize / 1024 / 1024} Mb";
            }

            return $"{totalSize / 1024 / 1024 / 1024} Gb";
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
