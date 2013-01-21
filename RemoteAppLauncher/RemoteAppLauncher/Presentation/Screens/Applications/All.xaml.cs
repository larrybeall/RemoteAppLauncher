using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RemoteAppLauncher.Presentation.Items;

namespace RemoteAppLauncher.Presentation.Screens.Applications
{
    /// <summary>
    /// Interaction logic for All.xaml
    /// </summary>
    public partial class All : UserControl
    {
        public All()
        {
            InitializeComponent();
        }

        private void SortedApplications_OnFilter(object sender, FilterEventArgs e)
        {
            var vm = DataContext as ApplicationsViewModel;
            if(vm == null || e.Item == null)
                return;

            if (string.IsNullOrEmpty(vm.SearchFilter))
            {
                e.Accepted = true;
                return;
            }

            if (e.Item.GetType() == typeof (DirectoryItemViewModel))
            {
                e.Accepted = false;
                return;
            }

            e.Accepted = ((FileItemViewModel) e.Item).Name.IndexOf(vm.SearchFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}
