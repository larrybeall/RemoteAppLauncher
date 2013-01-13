using Caliburn.Micro;

namespace RemoteAppLauncher.Presentation.Items
{
    public class DisplayEntryViewModel : ViewAware
    {
        private string _path;
        private string _name;
        private bool _isDirectory;
        
        public DisplayEntryViewModel()
            : this(false)
        {}

        public DisplayEntryViewModel(bool cacheViews)
            : base(cacheViews)
        {}

        public string Path
        {
            get { return _path; }
            set
            {
                if(_path == value) return;

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

        public bool IsDirectory
        {
            get { return _isDirectory; }
            set
            {
                if (_isDirectory == value) return;

                _isDirectory = value;
                NotifyOfPropertyChange(() => IsDirectory);
            }
        }
    }
}
