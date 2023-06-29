using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace L_Commander.UI.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            var valueStr = value.ToString().ToLower();
            var parameterStr = parameter.ToString().ToLower();

            if (valueStr == parameterStr)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = targetType.GenericTypeArguments.SingleOrDefault() ?? targetType;

            return value != null && value.Equals(true)
                ? Enum.Parse(type, parameter.ToString())
                : Binding.DoNothing;
        }
    }

    public class EnumNotEqualToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            var valueStr = value.ToString().ToLower();
            var parameterStr = parameter.ToString().ToLower();

            if (valueStr != parameterStr)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}