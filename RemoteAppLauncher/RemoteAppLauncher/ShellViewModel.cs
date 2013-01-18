using Caliburn.Micro;
using RemoteAppLauncher.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteAppLauncher.Presentation.Screens;
using RemoteAppLauncher.Infrastructure.Services;
using RemoteAppLauncher.Infrastructure.Events;

namespace RemoteAppLauncher
{
    public class ShellViewModel 
            : Conductor<IScreen>.Collection.OneActive, 
            IHandle<InitializationCompleteEvent>, 
            IHandle<ApplicationExecutedEvent>,
            IHandle<LoadingEvent>
    {
        private readonly UsageBasedViewModel _usageBasedViewModel;
        private readonly ApplicationsViewModel _applicationsViewModel;
        private readonly ApplicationService _fileService;
        private readonly IEventAggregator _events;

        private string _filterString;
        private double _originalWindowWidth;
        private bool _allAppsVisible;
        private bool _initializing;
        private string _viewContext;

        public ShellViewModel()
        {
            _events = EventService.Instance;
            _fileService = ApplicationService.Instance;
            _usageBasedViewModel = new UsageBasedViewModel();
            _applicationsViewModel = new ApplicationsViewModel();

            _events.Subscribe(this);
            //ViewState = "Pinned";
            ActivateItem(_applicationsViewModel);
        }

        public string ViewState
        {
            get { return _viewContext; }
            set
            {
                if (_viewContext == value) return;

                _viewContext = value;
                NotifyOfPropertyChange(() => ViewState);
            }
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
            ViewState = ApplicationsViewModel.AllViewState;
        }

        public void ShowPinnedApplications()
        {
            AllAppsVisible = true;
            if (_originalWindowWidth > 0 && _originalWindowWidth < App.Current.MainWindow.Width)
                App.Current.MainWindow.Width = _originalWindowWidth;

            ViewState = ApplicationsViewModel.PinnedViewState;
        }

        public void HideAllApplications()
        {
            AllAppsVisible = false;
            App.Current.MainWindow.Width = _originalWindowWidth;
            ViewState = "Pinned";
        }

        public void Handle(InitializationCompleteEvent message)
        {
            Initializing = false;
        }

        public void Handle(ApplicationExecutedEvent message)
        {
            if(!AllAppsVisible)
                return;

            ShowPinnedApplications();
        }

        internal void Reset()
        {
            ShowPinnedApplications();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Initializing = true;
        }

        public void Handle(LoadingEvent message)
        {
            if (message == null) return;

            Initializing = message.Loading;
        }
    }
}
