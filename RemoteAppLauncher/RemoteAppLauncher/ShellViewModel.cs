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
            IHandle<LoadingEvent>
    {
        private readonly ApplicationsViewModel _applicationsViewModel;
        private readonly IEventAggregator _events;

        private string _filterString;
        private double _originalWindowWidth;
        private bool _allAppsVisible;
        private bool _initializing;
        private string _viewContext;

        public ShellViewModel()
        {
            _events = EventService.Instance;
            _applicationsViewModel = new ApplicationsViewModel();

            _events.Subscribe(this);
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

        public void Handle(InitializationCompleteEvent message)
        {
            Initializing = false;
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
