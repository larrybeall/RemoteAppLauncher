using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Models
{
    internal class PersistedFileItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool PinnedToStartMenu { get; set; }
        public bool PinnedToQuickLaunch { get; set; }
        public long Accesses { get; set; }
    }
}
