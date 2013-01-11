using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Items
{
    public class TreeViewItemViewModel : ViewAware
    {
        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        private readonly ObservableCollection<TreeViewItemViewModel> _children;
        private readonly TreeViewItemViewModel _parent;
        private readonly bool _canHaveChildren;

        private bool _isExpanded;
        private bool _isSelected;

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : this(parent, lazyLoadChildren, false)
        {
            
        }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren, bool cacheViews)
            : base(cacheViews)
        {
            _parent = parent;
            _children = new ObservableCollection<TreeViewItemViewModel>();
            _canHaveChildren = lazyLoadChildren;

            if (lazyLoadChildren)
                _children.Add(DummyChild);
        }

        private TreeViewItemViewModel()
        {
            
        }

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        public bool CanHaveChildren
        {
            get { return _canHaveChildren; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    NotifyOfPropertyChange(() => IsExpanded);
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;

                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        protected virtual void LoadChildren()
        {
            
        }

    }
}
