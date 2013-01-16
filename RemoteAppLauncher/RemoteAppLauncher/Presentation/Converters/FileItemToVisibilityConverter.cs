using RemoteAppLauncher.Presentation.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteAppLauncher.Presentation.Converters
{
    public class FileItemToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FileItemViewModel fileItem = value as FileItemViewModel;
            if (fileItem == null)
                return value;

            return (fileItem.Pinned || fileItem.Accesses > 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
