using Caliburn.Micro;
using RemoteAppLauncher.Data;
using RemoteAppLauncher.Infrastructure.Services;
using RemoteAppLauncher.Presentation.Shell;

namespace RemoteAppLauncher.Infrastructure
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
            ApplicationService.Instance.UpdateStoredItems(true);
            ApplicationService.Instance.StartWindowMonitor();

            base.OnStartup(sender, e);
        }
    }
}
