using Caliburn.Micro;
using RemoteAppLauncher.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private string _searchString;
        private bool _allProgramsChecked;
        private readonly UsageBasedViewModel _usageBasedViewModel;
        private readonly BrowserViewModel _browserViewModel;

        public ShellViewModel()
        {
            _usageBasedViewModel = new UsageBasedViewModel();
            _browserViewModel = new BrowserViewModel();

            ActivateItem(_usageBasedViewModel);
        }

        public override string DisplayName
        {
            get { return "AppLauncher"; }
            set
            {
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set {
                if (_searchString == value) return;

                _searchString = value;
                NotifyOfPropertyChange(() => SearchString);
            }
        }

        public bool AllProgramsChecked
        {
            get { return _allProgramsChecked; }
            set 
            {
                if (_allProgramsChecked == value) return;

                _allProgramsChecked = value;
                NotifyOfPropertyChange(() => AllProgramsChecked);
                HandleAllProgramsCheckedChange(value);
            }
        }

        private void HandleAllProgramsCheckedChange(bool isChecked)
        {
            if (isChecked)
                ChangeActiveItem(_browserViewModel, false);
            else
                ChangeActiveItem(_usageBasedViewModel, false);
        }

        internal void Reset()
        {
            ChangeActiveItem(_usageBasedViewModel, false);
            if (AllProgramsChecked)
            {
                _allProgramsChecked = false;
                NotifyOfPropertyChange(() => AllProgramsChecked);
            }
        }
    }
}
