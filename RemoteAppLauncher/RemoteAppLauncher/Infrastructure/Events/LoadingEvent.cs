using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Infrastructure.Events
{
    public class LoadingEvent
    {
        public LoadingEvent()
        {
            
        }

        public LoadingEvent(bool loading)
        {
            Loading = loading;
        }

        public bool Loading { get; set; }
    }
}
