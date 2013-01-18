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
    /// Interaction logic for Pinned.xaml
    /// </summary>
    public partial class Pinned : UserControl
    {
        private ApplicationsViewModel _vm;

        public Pinned()
        {
            InitializeComponent();
        }

        private void SortedApplications_OnFilter(object sender, FilterEventArgs e)
        {
            if (_vm == null && DataContext != null)
            {
                _vm = (ApplicationsViewModel)DataContext;
            }

            if (_vm == null || e.Item == null)
                return;

            if (e.Item.GetType() == typeof(DirectoryItemViewModel))
            {
                e.Accepted = false;
                return;
            }

            FileItemViewModel fileItem = (FileItemViewModel)e.Item;
            if (!fileItem.Pinned && fileItem.Accesses == 0)
            {
                e.Accepted = false;
                return;
            }

            e.Accepted = string.IsNullOrEmpty(_vm.SearchFilter) || fileItem.Name.Contains(_vm.SearchFilter);
        }
    }
}
