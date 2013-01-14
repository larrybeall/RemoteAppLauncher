using System.Collections.ObjectModel;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteAppLauncher.Infrastructure.Services;
using RemoteAppLauncher.Presentation.Items;

namespace RemoteAppLauncher.Presentation.Screens
{
    public class AllApplicationsViewModel : Screen
    {
        private readonly PersistedItemService _fileService;
        private BindableCollection<ViewAware> _applications;

        public AllApplicationsViewModel()
        {
            _fileService = new PersistedItemService();
        }

        public BindableCollection<ViewAware> Applications
        {
            get { return _applications; }
        }

        protected override void OnActivate()
        {
            if (_applications == null || _applications.Count <= 0)
            {
                _fileService.GetAllItems((items) =>
                    {
                        List<ViewAware> data = new List<ViewAware>();
                        var files = items.Select(x => new FileItemViewModel(x));

                        var groups = files.OrderBy(x => x.Directory).GroupBy(x => x.Directory).ToList();

                        foreach (var fileGroup in groups)
                        {
                            if(fileGroup.Key.Equals("programs", StringComparison.InvariantCultureIgnoreCase))
                                continue;

                            data.Add(new DirectoryItemViewModel(fileGroup.Key));
                            data.AddRange(fileGroup);
                        }

                        var programs =
                            groups.SingleOrDefault(x => x.Key.Equals("programs", StringComparison.InvariantCultureIgnoreCase));
                        
                        if (programs != null)
                        {
                            data.InsertRange(0, programs);
                        }

                        _applications = new BindableCollection<ViewAware>(data);
                        NotifyOfPropertyChange(() => Applications);
                    });
            }

            base.OnActivate();
        }
    }
}
