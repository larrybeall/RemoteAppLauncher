using System.Diagnostics;
using RemoteAppLauncher.Data.Models;
using Caliburn.Micro;

namespace RemoteAppLauncher.Presentation.Items
{
    public class FileItemViewModel : DirectoryItemViewModel
    {
        private string _path;
        private string _name;
        private string _directory;

        public FileItemViewModel()
        {}

        public FileItemViewModel(PersistedFileItem entry)
            : base(entry.Name)
        {
            Path = entry.Path;
            Directory = entry.Directory;
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
