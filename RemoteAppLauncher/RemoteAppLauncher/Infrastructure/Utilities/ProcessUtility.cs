using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        public static string GetPathFromHandle(IntPtr windowHandle)
        {
            uint processId = 0;
            Interop.GetWindowThreadProcessId(windowHandle, out processId);
            if (processId <= 0)
            {
                return String.Empty;
            }

            var process = Process.GetProcessById((int) processId);
            return process.MainModule.FileName;
        }

        private class Interop
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);
        }
    }
}
