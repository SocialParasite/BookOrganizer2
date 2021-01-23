using System;
using System.Globalization;
using System.Windows.Data;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class SeriesBooksReadStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var totalCount = ((SeriesState) value)?.BookCount;
            var readCount = ((SeriesState)value)?.ReadBookCount;
            if (readCount > 0)
            {
                return ((decimal)readCount / (decimal)totalCount * 100);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
