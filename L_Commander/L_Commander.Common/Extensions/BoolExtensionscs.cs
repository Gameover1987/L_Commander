using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.Common.Extensions
{
    public static class BoolExtensionscs
    {
        public static bool Invert(this bool boolValue)
        {
            return !boolValue;
        }
    }
}
