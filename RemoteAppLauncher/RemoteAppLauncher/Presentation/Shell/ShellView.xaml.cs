using MahApps.Metro.Controls;
using System.Windows.Media;

namespace RemoteAppLauncher.Presentation.Shell
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        public ShellView()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            InitializeComponent();
        }
    }
}
