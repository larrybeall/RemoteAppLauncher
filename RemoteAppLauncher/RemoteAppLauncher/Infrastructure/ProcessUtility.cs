using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Infrastructure
{
    internal static class ProcessUtility
    {
        public static void ExecuteProcess(string executablePath)
        {
            if(string.IsNullOrEmpty(executablePath))
                throw new ArgumentNullException("executablePath");

            try
            {
                var p = Process.Start(executablePath);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                ProcessStartInfo psi = new ProcessStartInfo(executablePath);
                psi.UseShellExecute = false;
                Process.Start(psi);
            }
            
        }

        public static void OpenControlPanel()
        {
            ExecuteProcess(PathUtility.ControlPanelPath);
        }

        public static void OpenFileExplorer()
        {
            ExecuteProcess(PathUtility.FileExplorerPath);
        }
    }
}
