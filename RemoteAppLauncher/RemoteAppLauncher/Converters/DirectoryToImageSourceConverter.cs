﻿using RemoteAppLauncher.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteAppLauncher.Converters
{
    public class DirectoryToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool smallIcon = false;
            if (parameter != null)
                smallIcon = System.Convert.ToBoolean(parameter);

            if (smallIcon)
                return IconUtility.GetSmallFolderIcon();

            return IconUtility.GetLargeFolderIcon();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
