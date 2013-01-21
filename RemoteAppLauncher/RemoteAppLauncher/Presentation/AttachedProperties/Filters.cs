using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RemoteAppLauncher.Presentation.AttachedProperties
{
    public class Filters
    {
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.RegisterAttached("FilterText", 
            typeof (string), 
            typeof (Filters), 
            new UIPropertyMetadata(FilterChanged));

        public static void SetFilterText(DependencyObject element, string value)
        {
            element.SetValue(FilterTextProperty, value);
        }

        public static string GetFilterText(DependencyObject element)
        {
            return (string) element.GetValue(FilterTextProperty);
        }

        private static void FilterChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewSource = sender as CollectionViewSource;
            if (viewSource != null && viewSource.View != null)
            {
                viewSource.View.Refresh();
            }
        }
    }
}
