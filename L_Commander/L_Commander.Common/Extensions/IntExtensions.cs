using System;
using System.Collections.Generic;
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
                return $"~{totalSize / 1024} Kb";
            }

            if (totalSize < 1024 * 1024 * 1024)
            {
                return $"~{totalSize / 1024 / 1024} Mb";
            }

            return $"~{totalSize / 1024 / 1024 / 1024} Gb";
        }
    }
}
