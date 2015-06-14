using System;
using RemoteAppLauncher.Infrastructure.Services;
using RemoteAppLauncher.Infrastructure.Utilities;

namespace RemoteAppLauncher.Presentation.Items
{
    public class OpenItemViewModel : FileItemViewModel
    {
        internal readonly IntPtr Hwnd;

        private bool _active;
        private readonly DateTime _createTime;

        public OpenItemViewModel(IntPtr hwnd)
        {
            SetIconRetriever(IconUtility.GetLargeIcon);
            Hwnd = hwnd;
            _createTime = DateTime.Now;
        }

        public OpenItemViewModel(IntPtr hwnd, string name, string path)
            : this(hwnd)
        {
            SetIconRetriever(IconUtility.GetLargeIcon);
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

        public DateTime CreateTime
        {
            get { return _createTime; }
        }

        public void Activate()
        {
            ApplicationService.Instance.ActivateWindow(this);
        }
    }
}
