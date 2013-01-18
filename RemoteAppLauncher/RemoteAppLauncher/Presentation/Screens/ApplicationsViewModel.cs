using System.Collections.ObjectModel;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteAppLauncher.Infrastructure.Services;
using RemoteAppLauncher.Presentation.Items;
using System.Windows.Controls;
using System.Windows.Threading;
using RemoteAppLauncher.Infrastructure.Events;

namespace RemoteAppLauncher.Presentation.Screens
{
    public class ApplicationsViewModel : Screen, IHandle<ApplicationsChangedEvent>
    {
        public static string PinnedViewState = "Pinned";
        public static string AllViewState = "All";

        private readonly IEventAggregator _events;
        private readonly ApplicationService _applicationService;
        private List<ViewAware> _applications = new List<ViewAware>();
        private string _searchFilter;
        private double _originalWindowWidth;
        private bool _allAppsVisible;
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

        public bool AllAppsVisible
        {
            get { return _allAppsVisible; }
            set { _allAppsVisible = value; NotifyOfPropertyChange(() => AllAppsVisible); }
        }

        public void FileSelected(ListBox source)
        {
            if (source == null)
                return;

            var item = source.SelectedItem as FileItemViewModel;
            if (item == null)
                return;

            item.Execute();
            source.SelectedItem = null;

            Dispatcher.CurrentDispatcher.Invoke(() => ((ShellViewModel)Parent).Reset());
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
        }

        public void ShowAllApplications()
        {
            AllAppsVisible = true;
            ViewState = ApplicationsViewModel.AllViewState;
            _originalWindowWidth = App.Current.MainWindow.Width;
            App.Current.MainWindow.Width = 700;
        }

        public void ShowPinnedApplications()
        {
            AllAppsVisible = false;
            if (_originalWindowWidth > 0 && _originalWindowWidth < App.Current.MainWindow.Width)
                App.Current.MainWindow.Width = _originalWindowWidth;

            ViewState = ApplicationsViewModel.PinnedViewState;
        }
    }
}
