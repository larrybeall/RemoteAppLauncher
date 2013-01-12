using System.Collections;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteAppLauncher.Infrastructure;
using RemoteAppLauncher.Items;
using RemoteAppLauncher.Models;
using System.IO;

namespace RemoteAppLauncher.Screens
{
    public class BrowserViewModel : Screen
    {
        private readonly BindableCollection<TreeViewItemViewModel> _entries;  

        private bool _loading;

        public BrowserViewModel()
        {
            _entries = new BindableCollection<TreeViewItemViewModel>();
        }

        protected override void OnInitialize()
        {
            Loading = true;
            SynchronizationContext uiContext = SynchronizationContext.Current;

            Task getEntries = new Task(() =>
                {
                    string commonStartMenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                    string userStartMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);

                    commonStartMenu = Path.Combine(commonStartMenu, "Programs");
                    userStartMenu = Path.Combine(userStartMenu, "Programs");

                    IEnumerable<DirectoryEntry> entries = PathUtility.GetDistinctDirectoryEntries(commonStartMenu,
                                                                                                  userStartMenu);
                    var vms =
                        entries
                            .OrderByDescending(x => x.IsDirectory)
                            .ThenBy(x => x.Name)
                            .Select(x =>
                                {
                                    TreeViewItemViewModel vm;
                                    if(x.IsDirectory)
                                        vm = new DirectoryItemViewModel(x);
                                    else
                                        vm = new FileItemViewModel(x);

                                    return vm;
                                });

                    if (uiContext == null)
                        LoadEntries(vms);
                    else
                        uiContext.Post(_ => LoadEntries(vms), null);
                });

            getEntries.Start();
            base.OnInitialize();
        }

        public bool Loading
        {
            get { return _loading; }
            set { 
                _loading = value;
                NotifyOfPropertyChange(() => Loading);
            }
        }

        public BindableCollection<TreeViewItemViewModel> Entries
        {
            get { return _entries; }
        }

        private void LoadEntries(IEnumerable<TreeViewItemViewModel> entries)
        {
            _entries.AddRange(entries);
            Loading = false;
        }

        public void OnSelectionChanged(RoutedPropertyChangedEventArgs<object> args)
        {
            if(args.NewValue == null)
                return;

            ((TreeViewItemViewModel) args.NewValue).IsSelected = false;

            var fileItem = args.NewValue as FileItemViewModel;
            if(fileItem == null)
                return;

            fileItem.Execute();

            App.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var treeViewItemViewModel in Entries)
                    {
                        if (!treeViewItemViewModel.IsExpanded)
                            continue;

                        treeViewItemViewModel.CollapseAll();
                    }

                    ShellViewModel shell = Parent as ShellViewModel;
                    if (shell != null)
                        shell.Reset();
                });
        }
    }
}
