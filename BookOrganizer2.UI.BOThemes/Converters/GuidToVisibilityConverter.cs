﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookOrganizer2.UI.BOThemes.Converters
{
    [ValueConversion(typeof(bool), typeof(double))]
    public class GuidToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: 
            dynamic test = value;
            return value is not null && (Guid)test.Value == default
                ? Visibility.Collapsed
                : (object)Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
