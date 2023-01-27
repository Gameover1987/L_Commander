using System;
using System.Globalization;
using System.Windows.Data;
using L_Commander.Common.Extensions;

namespace L_Commander.App.Views.Converters;

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