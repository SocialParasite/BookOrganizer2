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
            switch (value)
            {
                case BookStatus:
                    return value switch
                    {
                        BookStatus.Read | BookStatus.Owned => new SolidColorBrush(Colors.PaleGreen),
                        BookStatus.Read => new SolidColorBrush(Colors.PaleVioletRed),
                        BookStatus.Owned => new SolidColorBrush(Colors.LightSkyBlue),
                        _ => new SolidColorBrush(Colors.WhiteSmoke),
                    };
                case SeriesStatus:
                    return value switch
                    {
                        SeriesStatus.NoneOwnedNoneRead => new SolidColorBrush(Colors.Red),
                        SeriesStatus.NoneOwnedPartlyRead => new SolidColorBrush(Colors.OrangeRed),
                        SeriesStatus.NoneOwnedAllRead => new SolidColorBrush(Colors.PaleVioletRed),
                        SeriesStatus.PartlyOwnedNoneRead => new SolidColorBrush(Colors.AliceBlue),
                        SeriesStatus.PartlyOwnedPartlyRead => new SolidColorBrush(Colors.LightSteelBlue),
                        SeriesStatus.PartlyOwnedAllRead => new SolidColorBrush(Colors.CornflowerBlue),
                        SeriesStatus.AllOwnedNoneRead => new SolidColorBrush(Colors.LightSkyBlue),
                        SeriesStatus.AllOwnedPartlyRead => new SolidColorBrush(Colors.DeepSkyBlue),
                        SeriesStatus.AllOwnedAllRead => new SolidColorBrush(Colors.PaleGreen),
                        _ => new SolidColorBrush(Colors.WhiteSmoke),
                    };
                default:
                    return new SolidColorBrush(Colors.WhiteSmoke);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        [Flags]
        public enum BookStatus
        {
            None = 0,
            Read = 1,
            Owned = 2
        }

        [Flags]
        public enum SeriesStatus
        {
            None = 0,
            AllBooksRead = 1,
            AllBooksOwned = 2,
            PartlyRead = 4,
            PartlyOwned = 8,
            NoneRead = 16,
            NoneOwned = 32,

            NoneOwnedNoneRead = NoneOwned | NoneRead,
            NoneOwnedPartlyRead = NoneOwned | PartlyRead,
            NoneOwnedAllRead = NoneOwned | AllBooksRead,

            PartlyOwnedNoneRead = PartlyOwned | NoneRead,
            PartlyOwnedPartlyRead = PartlyOwned | PartlyRead,
            PartlyOwnedAllRead = PartlyOwned | AllBooksRead,

            AllOwnedNoneRead = AllBooksOwned | NoneRead,
            AllOwnedPartlyRead = AllBooksOwned | PartlyRead,
            AllOwnedAllRead = AllBooksOwned | AllBooksRead
        }
    }
}
