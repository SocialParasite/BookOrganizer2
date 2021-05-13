using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class ItemsCountStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{values[1]} / {values[0]}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
