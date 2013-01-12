using RemoteAppLauncher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Items
{
    public class FileItemViewModel : TreeViewItemViewModel
    {
        private string _path;
        private string _name;

        public FileItemViewModel(DirectoryEntry entry, TreeViewItemViewModel parent = null)
            : base(parent, false)
        {
            Path = entry.Paths[0];
            Name = entry.Name;
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path == value) return;

                _path = value;
                NotifyOfPropertyChange(() => Path);
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

        public void Execute()
        {
            Process p = Process.Start(Path);
        }
    }
}
