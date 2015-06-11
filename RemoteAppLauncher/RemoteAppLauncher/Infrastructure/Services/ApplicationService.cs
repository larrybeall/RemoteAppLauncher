using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using RemoteAppLauncher.Data.Models;
using RemoteAppLauncher.Data.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Automation;
using System.Windows.Interop;
using ManagedWinapi.Accessibility;
using RemoteAppLauncher.Infrastructure.Utilities;
using RemoteAppLauncher.Presentation.Items;
using RemoteAppLauncher.Infrastructure.Events;
using Win32Interop.Methods;

namespace RemoteAppLauncher.Infrastructure.Services
{
    internal class ApplicationService
    {
        private static readonly object SyncRoot = new object();
        private readonly PersistedItemRepository _repository;
        private readonly SynchronizationContext _uiContext;
        private readonly IEventAggregator _events;

        private static volatile ApplicationService _instance;
        private static ConcurrentDictionary<string, FileItemViewModel> _applications = new ConcurrentDictionary<string, FileItemViewModel>();
        private static ConcurrentDictionary<IntPtr, OpenItemViewModel> _openWindows = new ConcurrentDictionary<IntPtr, OpenItemViewModel>();

        private AutomationEventHandler _windowEventHandler;

        private ApplicationService()
        {
            _repository = new PersistedItemRepository();
            _uiContext = SynchronizationContext.Current;
            _events = EventService.Instance;
        }

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

        public IEnumerable<OpenItemViewModel> OpenWindows
        {
            get { return _openWindows.Select(x => x.Value); }
        }

        public void UpdateStoredItems(bool initializing)
        {
            Task updateTask = new Task(() =>
                {
                    if (!initializing)
                    {
                        Thread.Sleep(1000);
                    }

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

                    if (!hasChangedItems)
                    {
                        if (_uiContext != null)
                            _uiContext.Post(_ => RaiseLoadingComplete(), null);
                        else
                            RaiseLoadingComplete();

                        return;
                    }

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
            _events.Publish(ApplicationExecutedEvent.Default);

            UpdateFromViewModel(file);
        }

        public void PinApp(FileItemViewModel fileVm)
        {
            UpdateFromViewModel(fileVm);
        }

        public void UnpinApp(FileItemViewModel fileVm)
        {
            UpdateFromViewModel(fileVm);
        }

        public void SetIconPath(FileItemViewModel fileVm)
        {
            UpdateFromViewModel(fileVm);
        }

        public void StartWindowMonitor()
        {
            _windowEventHandler = (sender, args) =>
            {
                var element = sender as AutomationElement;
                if (element == null)
                {
                    return;
                }

                var handle = new IntPtr(element.Current.NativeWindowHandle);
                HandleWindowOpenedEvent(handle);
            };

            Automation.AddAutomationEventHandler(
                WindowPattern.WindowOpenedEvent,
                AutomationElement.RootElement,
                TreeScope.Children,
                _windowEventHandler
              );

            var activationListener = new AccessibleEventListener
            {
                MinimalEventType = AccessibleEventType.EVENT_SYSTEM_FOREGROUND,
                MaximalEventType = AccessibleEventType.EVENT_SYSTEM_FOREGROUND,
                ThreadId = 0,
                ProcessId = 0
            };

            activationListener.EventOccurred += (sender, args) =>
            {
                HandleWindowActivatedEvent(args.HWnd);
            };

            activationListener.Enabled = true;

            var closedListener = new AccessibleEventListener
            {
                MinimalEventType = AccessibleEventType.EVENT_OBJECT_DESTROY,
                MaximalEventType = AccessibleEventType.EVENT_OBJECT_DESTROY,
                ThreadId = 0,
                ProcessId = 0
            };

            closedListener.EventOccurred += (sender, args) =>
            {
                if (args.ObjectID != (uint)AccessibleObjectID.OBJID_WINDOW)
                {
                    return;
                }

                if (args.ChildID != 0)
                {
                    return;
                }

                if (args.EventType == AccessibleEventType.EVENT_OBJECT_DESTROY)
                {
                    HandleWindowClosedEvent(args.HWnd);
                }
            };

            closedListener.Enabled = true;
        }

        private void NotifyNewItems(bool initializing)
        {
            if(initializing)
                RaiseInitializationComplete();

            RaiseApplicationsChanged();
        }

        private void RaiseInitializationComplete()
        {
            _events.Publish(InitializationCompleteEvent.Default);
        }

        private void RaiseApplicationsChanged()
        {
            _events.Publish(ApplicationsChangedEvent.Default);
        }

        private void RaiseLoadingComplete()
        {
            _events.Publish(new LoadingEvent(false));
        }

        private void UpdateFromViewModel(FileItemViewModel fileVm)
        {
            Task.Factory.StartNew(() =>
            {
                var persistedFile = new PersistedFileItem(fileVm);
                _repository.Update(persistedFile);
            });
        }

        private void HandleWindowActivatedEvent(IntPtr hwnd)
        {
            if (!_openWindows.ContainsKey(hwnd)) return;

            foreach (var openItem in _openWindows)
            {
                openItem.Value.Active = openItem.Key == hwnd;
            }
        }

        private void HandleWindowClosedEvent(IntPtr hwnd)
        {
            if (!_openWindows.ContainsKey(hwnd)) return;

            OpenItemViewModel removedItem;
            _openWindows.TryRemove(hwnd, out removedItem);
        }

        private void HandleWindowOpenedEvent(IntPtr hwnd)
        {
            if(Process.GetCurrentProcess().MainWindowHandle == hwnd) return;
            if (_openWindows.ContainsKey(hwnd)) return;

            var activatedWindow = User32.GetForegroundWindow();
            var active = activatedWindow == hwnd;

            StringBuilder windowTitleBuilder = new StringBuilder();
            User32.GetWindowText(hwnd, windowTitleBuilder, 300);

            var path = ProcessUtility.GetPathFromHandle(hwnd);

            var item = new OpenItemViewModel(hwnd, windowTitleBuilder.ToString(), path);
            _openWindows.TryAdd(hwnd, item);

            if (active)
            {
                HandleWindowActivatedEvent(hwnd);
            }
        }
    }
}
