using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Presentation.Items
{
    public class OpenItemViewModel : FileItemViewModel
    {
        internal readonly IntPtr Hwnd;

        private bool _active;

        public OpenItemViewModel(IntPtr hwnd)
        {
            Hwnd = hwnd;
        }

        public OpenItemViewModel(IntPtr hwnd, string name, string path)
            : this(hwnd)
        {
            Name = name;
            Path = path;
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                if(_active == value) return;

                _active = value;
                NotifyOfPropertyChange(() => Active);
            }
        }
    }
}
