using BookOrganizer2.Domain.BookProfile;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class BookInSeriesBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Book book = value as Book;
            var bookStatus = CheckBookStatus(book.IsRead, book.Formats.Count > 0);

            return bookStatus switch
            {
                BookStatus.Read | BookStatus.Owned => new SolidColorBrush(Colors.PaleGreen),
                BookStatus.Read => new SolidColorBrush(Colors.PaleVioletRed),
                BookStatus.Owned => new SolidColorBrush(Colors.LightSkyBlue),
                _ => new SolidColorBrush(Colors.WhiteSmoke),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static BookStatus CheckBookStatus(bool read, bool owned)
        {
            return read switch
            {
                true when owned => BookStatus.Read | BookStatus.Owned,
                true => BookStatus.Read,
                _ => owned ? BookStatus.Owned : BookStatus.None,
            };
        }
    }
}
