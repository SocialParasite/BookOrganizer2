using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class BookBackgroundColorMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRead = (bool)values[0];
            var isOwned = (values[1] as IList<Format>).Any();

            var bookStatus = CheckBookStatus(isRead, isOwned);

            return bookStatus switch
            {
                BookStatus.Read | BookStatus.Owned => new SolidColorBrush(Colors.PaleGreen),
                BookStatus.Read => new SolidColorBrush(Colors.PaleVioletRed),
                BookStatus.Owned => new SolidColorBrush(Colors.LightSkyBlue),
                _ => new SolidColorBrush(Colors.WhiteSmoke),
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
