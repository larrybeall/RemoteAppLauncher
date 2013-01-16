﻿using System.Collections.ObjectModel;
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
    public class AllApplicationsViewModel : Screen, IHandle<ApplicationsChangedEvent>
    {
        private readonly IEventAggregator _events;
        private readonly ApplicationService _applicationService;
        private List<ViewAware> _applications = new List<ViewAware>();

        public AllApplicationsViewModel()
        {
            _applicationService = ApplicationService.Instance;
            _events = EventService.Instance;
            _events.Subscribe(this);
        }

        public List<ViewAware> Applications
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
    }
}
