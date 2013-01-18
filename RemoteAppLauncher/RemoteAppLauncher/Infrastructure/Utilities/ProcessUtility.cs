using System;
using System.Diagnostics;

namespace RemoteAppLauncher.Infrastructure.Utilities
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
