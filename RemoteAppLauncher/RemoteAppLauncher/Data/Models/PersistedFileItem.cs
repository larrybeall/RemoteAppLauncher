using System;

namespace RemoteAppLauncher.Data.Models
{
    internal class PersistedFileItem
    {
        public PersistedFileItem()
        {}

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
                if (string.IsNullOrEmpty(_id))
                    _id = Path;

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

            return toCompare.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase)
                   && toCompare.Path.Equals(Path, StringComparison.InvariantCultureIgnoreCase)
                   && toCompare.Directory.Equals(Directory, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode()*17 + Path.GetHashCode()*17 + Directory.GetHashCode()*17;
        }
    }
}
