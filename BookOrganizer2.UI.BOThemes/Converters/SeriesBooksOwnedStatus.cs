using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class SeriesBooksOwnedStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var totalCount = ((SeriesState)value)?.BookCount;
            var ownedCount = ((SeriesState)value)?.OwnedBookCount;
            if (ownedCount > 0)
            {
                return ((decimal)ownedCount / (decimal)totalCount * 100);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
