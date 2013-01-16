using RemoteAppLauncher.Presentation.Items;
using System.Windows.Controls;
using System.Windows.Data;

namespace RemoteAppLauncher.Presentation.Screens
{
    /// <summary>
    /// Interaction logic for UsageBasedView.xaml
    /// </summary>
    public partial class UsageBasedView : UserControl
    {
        private UsageBasedViewModel _vm;

        public UsageBasedView()
        {
            InitializeComponent();
        }

        private void SortedApplications_OnFilter(object sender, FilterEventArgs e)
        {
            if (_vm == null && DataContext != null)
            {
                _vm = (UsageBasedViewModel) DataContext;
            }

            if(_vm == null || e.Item == null)
                return;

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
