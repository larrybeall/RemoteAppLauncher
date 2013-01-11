using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Models
{
    public class DirectoryEntry
    {
        public List<String> Paths { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
    }
}
