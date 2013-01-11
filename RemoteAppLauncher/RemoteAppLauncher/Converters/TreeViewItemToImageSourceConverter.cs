using RemoteAppLauncher.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteAppLauncher.Converters
{
    public class TreeViewItemToImageSourceConverter : IValueConverter
    {
        private readonly DirectoryToImageSourceConverter _directoryImageConverter;
        private readonly PathToImageSourceConverter _pathImageConverter;

        public TreeViewItemToImageSourceConverter()
        {
            _directoryImageConverter = new DirectoryToImageSourceConverter();
            _pathImageConverter = new PathToImageSourceConverter();
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is DirectoryItemViewModel)
                return _directoryImageConverter.Convert(value, targetType, parameter, culture);

            FileItemViewModel fileItem = value as FileItemViewModel;
            if (fileItem != null)
                return _pathImageConverter.Convert(fileItem.Path, targetType, parameter, culture);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
