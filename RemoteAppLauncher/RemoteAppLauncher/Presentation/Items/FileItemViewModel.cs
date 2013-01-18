using System.Diagnostics;
using RemoteAppLauncher.Data.Models;
using Caliburn.Micro;
using System.Windows.Media;
using RemoteAppLauncher.Infrastructure;
using System.Windows;
using RemoteAppLauncher.Infrastructure.Services;
using RemoteAppLauncher.Infrastructure.Utilities;

namespace RemoteAppLauncher.Presentation.Items
{
    public class FileItemViewModel : DirectoryItemViewModel
    {
        private readonly ApplicationService _applicationService;

        private string _path;
        private string _directory;
        private string _id;
        private bool _pinned;
        private long _accesses;

        public FileItemViewModel()
        {
        }

        public FileItemViewModel(PersistedFileItem entry)
            : base(entry.Name)
        {
            Path = entry.Path;
            Directory = entry.Directory;
            Pinned = entry.Pinned;
            Accesses = entry.Accesses;
        }

        public string Id
        {
            get { return _id; }
            set
            {
                if(_id == value) return;

                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path == value) return;

                _path = value;
                NotifyOfPropertyChange(() => Path);

                if (!string.IsNullOrEmpty(_path))
                {
                    ImageSource = IconUtility.GetLargeIcon(_path);
                    NotifyOfPropertyChange(() => ImageSource);
                }
            }
        }

        public ImageSource ImageSource { get; set; }

        public string Directory
        {
            get { return _directory; }
            set
            {
                if(_directory == value) return;

                _directory = value;
                NotifyOfPropertyChange(() => Directory);
            }
        }

        public bool Pinned
        {
            get { return _pinned; }
            set
            {
                if(_pinned == value)
                    return;

                _pinned = value;
                NotifyOfPropertyChange(() => Pinned);
            }
        }

        public long Accesses
        {
            get { return _accesses; }
            set
            {
                if(_accesses == value)
                    return;

                _accesses = value;
                NotifyOfPropertyChange(() => Accesses);
            }
        }

        public void Execute()
        {
            Accesses = Accesses + 1;

            ApplicationService.Instance.ExecuteApplication(this);
        }

        public void PinApp()
        {
            Pinned = true;

            ApplicationService.Instance.PinApp(this);
        }

        public void SetIconPath()
        {
            ApplicationService.Instance.SetIconPath(this);
        }
    }
}
