using System.Collections.Concurrent;
using System.Windows.Threading;
using RemoteAppLauncher.Data.Models;
using RemoteAppLauncher.Data.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace RemoteAppLauncher.Infrastructure.Services
{
    internal class PersistedItemService
    {
        private static readonly object SyncRoot = new object();
        private static ConcurrentBag<PersistedFileItem> _allItems = null;

        private readonly PersistedItemRepository _repository;
        private readonly DispatcherTimer _allItemsTimer;

        public PersistedItemService()
        {
            _repository = new PersistedItemRepository();
            _allItemsTimer = new DispatcherTimer();
            _allItemsTimer.Interval = TimeSpan.FromMinutes(5);
        }

        public void GetAllItems(Action<IEnumerable<PersistedFileItem>> onComplete)
        {
            if (_allItems != null && _allItems.Count > 0)
            {
                _allItemsTimer.Stop();
                _allItemsTimer.Start();

                onComplete(_allItems.ToList());
            }

            var callingContext = SynchronizationContext.Current;
            Task getAllTask = new Task(() =>
                {
                    var allItems = _repository.GetAll().ToList();

                    if (callingContext != null)
                        callingContext.Post(_ => RefreshAllItems(allItems, onComplete), null);
                    else
                        RefreshAllItems(allItems, onComplete);
                });

            getAllTask.Start();
        }

        public void UpdateStoredItems(Action onComplete)
        {
            var callingContext = SynchronizationContext.Current;
            Task updateTask = new Task(() =>
                {
                    var startMenuItems = PathUtility.GetAllFileEntries().Select(x => new PersistedFileItem(x)).ToList();

                    using (var cn = _repository.GetConnection())
                    {
                        cn.Open();

                        var persistedItems = _repository.GetAll(cn).ToList();

                        var newItems = startMenuItems.Except(persistedItems).ToList();
                        var removedItems = persistedItems.Except(startMenuItems).ToList();

                        bool refreshAllItems = false;

                        if (removedItems.Any())
                        {
                            _repository.Remove(removedItems);
                            refreshAllItems = true;
                        }

                        if (newItems.Any())
                        {
                            _repository.Insert(newItems);
                            refreshAllItems = true;
                        }

                        cn.Close();

                        if (callingContext != null)
                        {
                            callingContext.Post(_ =>
                                {
                                    if(refreshAllItems)
                                        ClearAllItems();

                                    onComplete();
                                }, null);
                        }
                        else
                        {
                            if(refreshAllItems)
                                ClearAllItems();

                            onComplete();
                        }
                    }
                });

            updateTask.Start();
        }

        private void ClearAllItems()
        {
            lock (SyncRoot)
            {
                _allItems = new ConcurrentBag<PersistedFileItem>();
            }
        }

        private void RefreshAllItems(ICollection<PersistedFileItem> items, Action<IEnumerable<PersistedFileItem>> callback)
        {
            _allItems = new ConcurrentBag<PersistedFileItem>(items);
            callback(items);

            _allItemsTimer.Tick += Tick;
            _allItemsTimer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            _allItemsTimer.Tick -= Tick;
            _allItemsTimer.Stop();

            ClearAllItems();
        }
    }
}
