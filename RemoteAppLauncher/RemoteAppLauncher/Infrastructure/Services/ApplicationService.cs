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
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows.Automation;
using System.Windows.Interop;
using ManagedWinapi.Accessibility;
using RemoteAppLauncher.Infrastructure.Utilities;
using RemoteAppLauncher.Presentation.Items;
using RemoteAppLauncher.Infrastructure.Events;
using Win32Interop.Enums;
using Win32Interop.Methods;
using Win32Interop.Structs;

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
                        ExecuteOnUiThread(RaiseLoadingComplete);
                        return;
                    }

                    ExecuteOnUiThread(() => NotifyNewItems(initializing));
                });

            updateTask.Start();
        }

        public void ExecuteApplication(FileItemViewModel file)
        {
            ProcessUtility.ExecuteProcess(file.Path);
            ExecuteOnUiThread(() => _events.Publish(ApplicationExecutedEvent.Default));

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
            var windows = new List<IntPtr>();
            EnumWindowsProc ewDelegate = (wnd, param) =>
            {
                if (wnd == IntPtr.Zero
                    || !ShouldIncludeWindow(wnd))
                {
                    return true;
                }

                //HandleWindowOpenedEvent(wnd);
                windows.Add(wnd);
                return true;
            };

            EnumWindows(ewDelegate, IntPtr.Zero);

            windows.ForEach(HandleWindowOpenedEvent);

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

            var titleChangeListener = new AccessibleEventListener
            {
                MinimalEventType = AccessibleEventType.EVENT_OBJECT_NAMECHANGE,
                MaximalEventType = AccessibleEventType.EVENT_OBJECT_NAMECHANGE,
                ThreadId = 0,
                ProcessId = 0
            };

            titleChangeListener.EventOccurred += (sender, args) =>
            {
                if (!_openWindows.ContainsKey(args.HWnd) || args.ObjectID != (uint) AccessibleObjectID.OBJID_WINDOW)
                {
                    return;
                }

                StringBuilder titleBuilder = new StringBuilder{Length = 300};
                User32.GetWindowText(args.HWnd, titleBuilder, 300);

                if (_openWindows.ContainsKey(args.HWnd))
                {
                    _openWindows[args.HWnd].Name = titleBuilder.ToString();
                }
            };

            titleChangeListener.Enabled = true;
        }

        public void ActivateWindow(OpenItemViewModel openWindowVm)
        {
            User32.SetForegroundWindow(openWindowVm.Hwnd);
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

        private void RaiseOpenWindowsChanged()
        {
            _events.Publish(OpenWindowsChangedEvent.Default);
        }

        private void UpdateFromViewModel(FileItemViewModel fileVm)
        {
            Task.Factory.StartNew(() =>
            {
                var persistedFile = new PersistedFileItem(fileVm);
                _repository.Update(persistedFile);
            });
        }

        private void HandleWindowActivatedEvent(IntPtr hwnd, bool publishEvent = true)
        {
            if (!_openWindows.ContainsKey(hwnd)) return;

            foreach (var openItem in _openWindows)
            {
                openItem.Value.Active = openItem.Key == hwnd;
            }

            if (publishEvent)
            {
                ExecuteOnUiThread(() => _events.Publish(WindowActivatedEvent.Default));
            }
        }

        private void HandleWindowClosedEvent(IntPtr hwnd)
        {
            if (!_openWindows.ContainsKey(hwnd)) return;

            OpenItemViewModel removedItem;
            _openWindows.TryRemove(hwnd, out removedItem);
            ExecuteOnUiThread(RaiseOpenWindowsChanged);
        }

        private void HandleWindowOpenedEvent(IntPtr hwnd)
        {
            if (Process.GetCurrentProcess().MainWindowHandle == hwnd) return;
            if (_openWindows.ContainsKey(hwnd)) return;

            var activatedWindow = User32.GetForegroundWindow();
            var active = activatedWindow == hwnd;

            StringBuilder windowTitleBuilder = new StringBuilder {Length = 300};
            User32.GetWindowText(hwnd, windowTitleBuilder, 300);

            var path = ProcessUtility.GetPathFromHandle(hwnd);

            var item = new OpenItemViewModel(hwnd, windowTitleBuilder.ToString(), path);
            _openWindows.TryAdd(hwnd, item);

            if (active)
            {
                HandleWindowActivatedEvent(hwnd, false);
            }
            ExecuteOnUiThread(RaiseOpenWindowsChanged);
        }

        private void ExecuteOnUiThread(System.Action toExecute)
        {
            if (_uiContext != null)
                _uiContext.Post(_ => toExecute(), null);
            else
                RaiseLoadingComplete();
        }

        private bool ShouldIncludeWindow(IntPtr hwnd)
        {
            if (!User32.IsWindowVisible(hwnd))
            {
                return false;
            }

            IntPtr hwndWalk = IntPtr.Zero;
            var hwndTry = User32.GetAncestor(hwnd, GA.GA_ROOTOWNER);
            while (hwndTry != hwndWalk)
            {
                hwndWalk = hwndTry;
                hwndTry = User32.GetLastActivePopup(hwndWalk);
                if (User32.IsWindowVisible(hwndTry))
                {
                    break;
                }
            }
            if (hwndWalk != hwnd)
            {
                return false;
            }

            TITLEBARINFO titleBarInfo = new TITLEBARINFO();
            User32.GetTitleBarInfo(hwnd, ref titleBarInfo);
            if ((titleBarInfo.rgstate[0] & STATE_SYSTEM_INVISIBLE) == 1)
            {
                return false;
            }

            //if ((User32.GetWindowLong(hwnd, GWL_EXSTYLE) & (uint)WS_EX.WS_EX_TOOLWINDOW) == 1)
            //{
            //    return false;
            //}

            if (((uint) User32.GetWindowLong(hwnd, GWL_STYLE) &
                 ((uint) WS.WS_BORDER | (uint) WS.WS_CHILD | (uint) WS.WS_VISIBLE)) !=
                ((uint) WS.WS_BORDER | (uint) WS.WS_VISIBLE))
            {
                return false;
            }

            return true;
        }

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private uint STATE_SYSTEM_INVISIBLE = 0x00008000;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;
    }
}
