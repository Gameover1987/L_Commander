using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using L_Commander.Common.Extensions;

namespace L_Commander.App.Views.Converters
{
    public class TotalSizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long totalSize = (long)value;
            if (totalSize == -1)
                return string.Empty;

            if (totalSize == 0)
                return totalSize.ToString();

            var numberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = " " };
            var totalSizeStr = String.Format(numberFormatInfo, "{0:#,#}", totalSize);

            return totalSizeStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TotalSizeToStringWithUnitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long totalSize = (long)value;
            return totalSize.SizeAsString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
