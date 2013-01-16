using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using Caliburn.Micro;
using RemoteAppLauncher.Presentation.Items;
using RemoteAppLauncher.Infrastructure.Services;
using System.Windows.Data;
using RemoteAppLauncher.Infrastructure.Events;

namespace RemoteAppLauncher.Presentation.Screens
{
    public class UsageBasedViewModel : Screen, IHandle<ApplicationsChangedEvent>
    {
        private readonly ApplicationService _applicationService;
        private readonly IEventAggregator _events;

        private ObservableCollection<FileItemViewModel> _applications;
        private string _searchFilter;

        public UsageBasedViewModel()
        {
            _events = EventService.Instance;
            _applications = new ObservableCollection<FileItemViewModel>();
            _applicationService = ApplicationService.Instance;

            _events.Subscribe(this);
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

        public IEnumerable<FileItemViewModel> Applications
        {
            get { return _applications; }
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
        }

        public void FilterApplications(FilterEventArgs args)
        {
            
        }

        public void Handle(ApplicationsChangedEvent message)
        {
            _applications = new ObservableCollection<FileItemViewModel>(_applicationService.Applications);
            NotifyOfPropertyChange(() => Applications);
        }
    }
}
