using System;
using System.Globalization;
using System.Windows.Data;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class ShowBordersInEditMode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null && ((bool)value) ? 1 : 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
