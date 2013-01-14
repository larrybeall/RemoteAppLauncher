using System.Collections.Generic;
using System.Linq;
using RemoteAppLauncher.Data.Models;
using RemoteAppLauncher.Infrastructure;
using Caliburn.Micro;

namespace RemoteAppLauncher.Presentation.Items
{
    public class DirectoryItemViewModel : ViewAware
    {
        private string _name;

        public DirectoryItemViewModel()
        {
            
        }

        public DirectoryItemViewModel(string name)
        {
            Name = name;
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
    }
}
