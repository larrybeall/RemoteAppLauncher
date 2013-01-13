using System.Collections.Generic;
using System.Linq;
using RemoteAppLauncher.Data.Models;
using RemoteAppLauncher.Infrastructure;

namespace RemoteAppLauncher.Presentation.Items
{
    public class DirectoryItemViewModel : TreeViewItemViewModel
    {
        private string _name;
        private List<string> _paths; 

        public DirectoryItemViewModel(DirectoryEntry entry, TreeViewItemViewModel parent = null)
            : base(parent, true)
        {
            Name = entry.Name;
            Paths = entry.Paths;
        }

        public List<string> Paths
        {
            get { return _paths; }
            set
            {
                if (_paths == value) return;

                _paths = value;
                NotifyOfPropertyChange(() => Paths);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        protected override void LoadChildren()
        {
            var entries = PathUtility.GetDistinctDirectoryEntries(_paths.ToArray());
            foreach (var directoryEntry in entries.OrderByDescending(x => x.IsDirectory).ThenBy(x => x.Name))
            {
                TreeViewItemViewModel entryToAdd;
                if (directoryEntry.IsDirectory)
                    entryToAdd = new DirectoryItemViewModel(directoryEntry, this);
                else
                    entryToAdd = new FileItemViewModel(directoryEntry, this);

                Children.Add(entryToAdd);
            }
        }
    }
}
