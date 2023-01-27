using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace L_Commander.UI.Converters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public EnumToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            var valueStr = value.ToString().ToLower();
            var parameterStr = parameter.ToString().ToLower();

            if (valueStr == parameterStr)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}