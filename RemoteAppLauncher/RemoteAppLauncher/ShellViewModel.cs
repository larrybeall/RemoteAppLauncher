using Caliburn.Micro;
using RemoteAppLauncher.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteAppLauncher.Presentation.Screens;
using RemoteAppLauncher.Infrastructure.Services;

namespace RemoteAppLauncher
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private string _filterString;
        private readonly UsageBasedViewModel _usageBasedViewModel;
        private readonly AllApplicationsViewModel _allApplicationsViewModel;
        private readonly ApplicationService _fileService;
        private double _originalWindowWidth;
        private bool _allAppsVisible;
        private bool _initializing;

        public ShellViewModel()
        {
            _fileService = ApplicationService.Instance;
            _fileService.InitializationComplete += (sender, args) => { Initializing = false; };
            _usageBasedViewModel = new UsageBasedViewModel();
            _allApplicationsViewModel = new AllApplicationsViewModel();

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

        public bool Initializing
        {
            get { return _initializing; }
            set { 
                _initializing = value;
                NotifyOfPropertyChange(() => Initializing);
            }
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
            _originalWindowWidth = App.Current.MainWindow.Width;
            App.Current.MainWindow.Width = 700;
            ChangeActiveItem(_allApplicationsViewModel, false);
        }

        public void HideAllApplications()
        {
            AllAppsVisible = false;
            App.Current.MainWindow.Width = _originalWindowWidth;
            ChangeActiveItem(_usageBasedViewModel, false);
        }

        internal void Reset()
        {
            HideAllApplications();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Initializing = true;
        }
    }
}
