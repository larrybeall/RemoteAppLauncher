using RemoteAppLauncher.Infrastructure;
using RemoteAppLauncher.Infrastructure.Utilities;
using RemoteAppLauncher.Presentation.Items;
using System;

namespace RemoteAppLauncher.Data.Models
{
    public class PersistedFileItem
    {
        public PersistedFileItem()
        {}

        public PersistedFileItem(FileItemViewModel vm)
        {
            Id = vm.Id;
            Name = vm.Name;
            Path = vm.Path;
            Directory = vm.Directory;
            Pinned = vm.Pinned;
            Accesses = vm.Accesses;
        }

        public PersistedFileItem(DirectoryEntry entry)
        {
            Name = entry.Name;
            Path = entry.Paths[0];
            Directory = entry.ParentDirectory;
        }

        private string _id;

        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id) && !string.IsNullOrEmpty(Path))
                    _id = HashUtility.Md5FromString(Path + Name + Directory);

                return _id;
            }
            set { _id = value; }
        }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Directory { get; set; }
        public bool Pinned { get; set; }
        public long Accesses { get; set; }

        public override bool Equals(object obj)
        {
            var toCompare = obj as PersistedFileItem;
            if (toCompare == null)
                return false;

            return toCompare.Id.Equals(Id, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
