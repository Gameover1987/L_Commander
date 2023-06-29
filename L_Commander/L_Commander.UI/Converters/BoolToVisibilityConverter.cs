using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace L_Commander.UI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool?)value;
            if (boolValue == true)
                return Visibility.Visible;

            if (parameter != null && parameter.ToString() == "Hidden")
                return Visibility.Hidden;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility) && (Visibility.Visible == (Visibility)value);
        }
    }
}
