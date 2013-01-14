using Caliburn.Micro;
using RemoteAppLauncher.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher
{
    public class AppBootstrapper : Bootstrapper<ShellViewModel>
    {
        public AppBootstrapper()
        {
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            // init the config manager, which will init the database if it doesn't exist.
            var config = DataConfigManager.Instance;

            base.OnStartup(sender, e);
        }
    }
}
