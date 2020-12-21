using System;
using System.Globalization;
using System.Windows.Data;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class BookSeriesHeader : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"Book Series ({value})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
