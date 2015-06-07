using Caliburn.Micro;
using RemoteAppLauncher.Infrastructure.Events;
using RemoteAppLauncher.Infrastructure.Services;
using RemoteAppLauncher.Presentation.Items;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RemoteAppLauncher.Presentation.Screens
{
    public class ApplicationsViewModel : Screen, IHandle<ApplicationsChangedEvent>, IHandle<ApplicationExecutedEvent>
    {
        public static string PinnedViewState = "Pinned";
        public static string AllViewState = "All";

        private readonly IEventAggregator _events;
        private readonly ApplicationService _applicationService;
        private List<ViewAware> _applications = new List<ViewAware>();
        private string _searchFilter;
        private double _originalWindowWidth;
        private string _viewContext;

        public ApplicationsViewModel()
        {
            _applicationService = ApplicationService.Instance;
            _events = EventService.Instance;
            _events.Subscribe(this);
        }

        public List<ViewAware> Applications
        {
            get { return _applications; }
        }

        public string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                if (_searchFilter == value) return;

                _searchFilter = value;
                NotifyOfPropertyChange(() => SearchFilter);
            }
        }

        public string ViewState
        {
            get
            {
                if (string.IsNullOrEmpty(_viewContext))
                    _viewContext = PinnedViewState;

                return _viewContext;
            }
            set
            {
                if (_viewContext == value) return;

                _viewContext = value;
                NotifyOfPropertyChange(() => ViewState);
            }
        }

        public void FileSelected(ListBox source)
        {
            if (source == null)
                return;

            source.SelectedItem = null;
        }

        public void Handle(ApplicationsChangedEvent message)
        {
            var applications =
                _applicationService.Applications
                                   .OrderBy(x => x.Directory)
                                   .ThenBy(x => x.Name)
                                   .GroupBy(x => x.Directory)
                                   .ToList();

            List<ViewAware> newApplicationList = new List<ViewAware>();
            foreach (var applicationGroup in applications)
            {
                newApplicationList.Add(new DirectoryItemViewModel(applicationGroup.Key));
                newApplicationList.AddRange(applicationGroup);
            }

            _applications = newApplicationList;
            NotifyOfPropertyChange(() => Applications);

            _events.Publish(new LoadingEvent(false));
        }

        public void ShowAllApplications()
        {
            ViewState = AllViewState;
            _originalWindowWidth = Application.Current.MainWindow.Width;
            Application.Current.MainWindow.Width = 700;
        }

        public void ShowPinnedApplications()
        {
            if (_originalWindowWidth > 0 && _originalWindowWidth < Application.Current.MainWindow.Width)
                Application.Current.MainWindow.Width = _originalWindowWidth;

            ViewState = PinnedViewState;
            SearchFilter = string.Empty;
        }

        public void RefreshApplications()
        {
            _events.Publish(new LoadingEvent(true));
            ApplicationService.Instance.UpdateStoredItems(false);
        }

        public void Handle(ApplicationExecutedEvent message)
        {
            if (!string.IsNullOrEmpty(SearchFilter))
                SearchFilter = string.Empty;

            if (ViewState != AllViewState)
                return;

            ShowPinnedApplications();
        }
    }
}
