using System.Diagnostics;
using RemoteAppLauncher.Data.Models;
using Caliburn.Micro;
using System.Windows.Media;
using RemoteAppLauncher.Infrastructure;

namespace RemoteAppLauncher.Presentation.Items
{
    public class FileItemViewModel : DirectoryItemViewModel
    {
        private string _path;
        private string _directory;
        private string _id;

        public FileItemViewModel()
        {}

        public FileItemViewModel(PersistedFileItem entry)
            : base(entry.Name)
        {
            Path = entry.Path;
            Directory = entry.Directory;
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

        public void Execute()
        {
            Process p = Process.Start(Path);
        }
    }
}
