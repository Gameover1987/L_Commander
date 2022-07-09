using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace L_Commander.App.Views.Converters
{
    public class TotalSizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long totalSize = (long)value;
            if (totalSize == 0)
                return string.Empty;

            var numberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = " " };
            var totalSizeStr = String.Format(numberFormatInfo, "{0:#,#}", totalSize);

            return totalSizeStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
