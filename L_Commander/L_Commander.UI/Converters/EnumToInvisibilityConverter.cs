using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace L_Commander.UI.Converters
{
    public class EnumToInvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return Binding.DoNothing;

                return value.Equals(parameter) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}