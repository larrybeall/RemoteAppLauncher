using Caliburn.Micro;
using RemoteAppLauncher.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteAppLauncher.Presentation.Screens;

namespace RemoteAppLauncher
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private string _filterString;
        private readonly UsageBasedViewModel _usageBasedViewModel;
        private readonly BrowserViewModel _browserViewModel;
        private bool _allAppsVisible;

        public ShellViewModel()
        {
            _usageBasedViewModel = new UsageBasedViewModel();
            _browserViewModel = new BrowserViewModel();

            ActivateItem(_usageBasedViewModel);
        }

        public override string DisplayName
        {
            get { return "AppLauncher"; }
            set
            {
            }
        }

        public string FilterString
        {
            get { return _filterString; }
            set {
                if (_filterString == value) return;

                _filterString = value;
                NotifyOfPropertyChange(() => FilterString);
            }
        }

        public bool AllAppsVisible
        {
            get { return _allAppsVisible; }
            set { _allAppsVisible = value; NotifyOfPropertyChange(() => AllAppsVisible); }
        }

        public void OpenControlPanel()
        {
            ProcessUtility.OpenControlPanel();
        }

        public void OpenFileExplorer()
        {
            ProcessUtility.OpenFileExplorer();
        }

        public void ShowAllApplications()
        {
            AllAppsVisible = true;
            ChangeActiveItem(_browserViewModel, false);
        }

        public void HideAllApplications()
        {
            AllAppsVisible = false;
            ChangeActiveItem(_usageBasedViewModel, false);
        }

        internal void Reset()
        {
            HideAllApplications();
        }
    }
}
