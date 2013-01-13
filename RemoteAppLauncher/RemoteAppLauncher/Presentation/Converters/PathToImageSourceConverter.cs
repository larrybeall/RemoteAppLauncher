using RemoteAppLauncher.Infrastructure;
using System;
using System.IO;
using System.Windows.Data;

namespace RemoteAppLauncher.Presentation.Converters
{
    public class PathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = value as string;
            if (string.IsNullOrEmpty(path) && File.Exists(path))
                return null;

            bool smallIcon = false;
            if (parameter != null)
                smallIcon = System.Convert.ToBoolean(parameter);

            if (smallIcon)
                return IconUtility.GetSmallIcon(path);

            return IconUtility.GetLargeIcon(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
