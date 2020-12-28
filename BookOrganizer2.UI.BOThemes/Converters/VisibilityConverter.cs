using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    public class VisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue && values[1] == DependencyProperty.UnsetValue) return Visibility.Collapsed;
            if (values[0] == DependencyProperty.UnsetValue && (bool)values[1]) return Visibility.Collapsed;
            if (values[0] != DependencyProperty.UnsetValue && (int)values[0] == 0 && (bool)values[1]) return Visibility.Collapsed;
            if (values[0] != DependencyProperty.UnsetValue || !(bool)values[1]) return Visibility.Visible;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
