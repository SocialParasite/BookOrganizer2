using BookOrganizer2.Domain.BookProfile;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
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
    }
}
