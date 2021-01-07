using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class BoolToMaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value 
                ? new GridLength(1, GridUnitType.Star) 
                : new GridLength(0, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
