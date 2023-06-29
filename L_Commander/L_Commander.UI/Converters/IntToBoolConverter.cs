using System;
using System.Globalization;
using System.Windows.Data;
using L_Commander.Common.Extensions;

namespace L_Commander.UI.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool) value;
            if (boolValue)
            {
                return parameter.ToInt();
            }

            return Binding.DoNothing;
        }
    }
}
