namespace RemoteAppLauncher.Data.Models
{
    internal class PersistedFileItem
    {
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
        public bool Pinned { get; set; }
        public long Accesses { get; set; }
    }
}
