using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Ruminoid.Common2.Metro.Utils.Converters
{
    public class FloorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                decimal d => Math.Floor(d),
                double d => Math.Floor(d),
                _ => throw new InvalidCastException($"Invalid type {value.GetType()}")
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new InvalidCastException();
    }
}
