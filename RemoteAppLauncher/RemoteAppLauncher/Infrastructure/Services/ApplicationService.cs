using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Threading;
using RemoteAppLauncher.Data.Models;
using RemoteAppLauncher.Data.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using RemoteAppLauncher.Presentation.Items;

namespace RemoteAppLauncher.Infrastructure.Services
{
    internal class ApplicationService
    {
        private static readonly object SyncRoot = new object();
        private readonly PersistedItemRepository _repository;
        private readonly SynchronizationContext _uiContext;

        private static volatile ApplicationService _instance;
        private static ConcurrentDictionary<string, FileItemViewModel> _applications = new ConcurrentDictionary<string, FileItemViewModel>();

        private ApplicationService()
        {
            _repository = new PersistedItemRepository();
            _uiContext = SynchronizationContext.Current;
        }

        public event EventHandler InitializationComplete;
        public event EventHandler ApplicationsChanged;

        public static ApplicationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if(_instance == null)
                            _instance = new ApplicationService();
                    }
                }
                return _instance;
            }
        }

        public IEnumerable<FileItemViewModel> Applications
        {
            get { return _applications.Select(x => x.Value); }
        }

        public void UpdateStoredItems(bool initializing)
        {
            Task updateTask = new Task(() =>
                {
                    var startMenuItems = PathUtility.GetAllFileEntries().Select(x => new PersistedFileItem(x)).ToList();

                    bool hasChangedItems = false;
                    List<PersistedFileItem> newItems = null;
                    List<PersistedFileItem> removedItems = null;

                    using (var cn = _repository.GetConnection())
                    {
                        cn.Open();

                        var persistedItems = _repository.GetAll(cn).ToList();

                        newItems = startMenuItems.Except(persistedItems).ToList();
                        removedItems = persistedItems.Except(startMenuItems).ToList();

                        if (removedItems.Any())
                        {
                            _repository.Remove(removedItems);
                            hasChangedItems = true;
                        }

                        if (newItems.Any())
                        {
                            _repository.Insert(newItems);
                            hasChangedItems = true;
                        }

                        if (initializing)
                        {
                            newItems = _repository.GetAll(cn).ToList();
                            hasChangedItems = true;
                        }

                        cn.Close();
                    }

                    foreach (var newItem in newItems)
                    {
                        _applications.TryAdd(newItem.Id, new FileItemViewModel(newItem));
                    }

                    foreach (var toRemove in removedItems)
                    {
                        FileItemViewModel removedItem;
                        _applications.TryRemove(toRemove.Id, out removedItem);
                    }

                    if(!hasChangedItems)
                        return;

                    if (_uiContext != null)
                        _uiContext.Post(_ => NotifyNewItems(initializing), null);
                    else
                        NotifyNewItems(initializing);
                });

            updateTask.Start();
        }

        public void ExecuteApplication(FileItemViewModel file)
        {
            ProcessUtility.ExecuteProcess(file.Path);
        }

        public void PinApp(FileItemViewModel file)
        {
            MessageBox.Show("Pin App");
        }

        public void SetIconPath(FileItemViewModel file)
        {
            MessageBox.Show("Set Icon Path");
        }

        private void NotifyNewItems(bool initializing)
        {
            if(initializing)
                RaiseInitializationComplete();

            RaiseApplicationsChanged();
        }

        private void RaiseInitializationComplete()
        {
            var handler = InitializationComplete;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void RaiseApplicationsChanged()
        {
            var handler = ApplicationsChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
