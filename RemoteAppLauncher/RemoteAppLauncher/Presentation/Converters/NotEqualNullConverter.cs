﻿using System;
using System.Windows.Data;

namespace RemoteAppLauncher.Presentation.Converters
{
    public class NotEqualNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value);
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
