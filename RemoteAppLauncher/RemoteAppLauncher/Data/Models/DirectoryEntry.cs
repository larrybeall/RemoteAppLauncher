using System;
using System.Collections.Generic;

namespace RemoteAppLauncher.Data.Models
{
    public class DirectoryEntry
    {
        public List<String> Paths { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
    }
}
