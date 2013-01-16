using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Infrastructure.Services
{
    public class EventService : EventAggregator
    {
        private static readonly object SyncRoot = new object();

        private static volatile EventService _instance;

        private EventService()
            : base()
        {}

        public static EventService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if(_instance == null)
                            _instance = new EventService();
                    }
                }

                return _instance;
            }
        }
    }
}
