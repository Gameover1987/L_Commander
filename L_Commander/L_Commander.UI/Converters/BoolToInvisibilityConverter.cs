using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace L_Commander.UI.Converters
{
    public class BoolToInvisibilityConverter : IValueConverter
    {
        public BoolToInvisibilityConverter()
            : base()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool?)value;
            if (boolValue == false)
                return Visibility.Visible;

            if (parameter != null && parameter.ToString() == "Hidden")
                return Visibility.Hidden;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
