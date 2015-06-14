using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using RemoteAppLauncher.Infrastructure.Events;
using RemoteAppLauncher.Presentation.Items;

namespace RemoteAppLauncher.Presentation.Converters
{
    public class ActiveColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isActive;

            var itemCollection = values[0] as IEnumerable<object>;
            if (itemCollection != null)
            {
                isActive = IsAnyActive(itemCollection);
            }
            else
            {
                isActive = IsActive(values[0]);
            }

            if (isActive)
            {
                return values[1];
            }

            return values[2];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool IsAnyActive(IEnumerable<Object> items)
        {
            return items.OfType<OpenItemViewModel>().Any(x => x.Active);
        }

        private bool IsActive(object activeObject)
        {
            return (bool) activeObject;
        }
    }
}
