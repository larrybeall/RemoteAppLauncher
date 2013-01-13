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

            Process.Start(executablePath);
        }

        public static void OpenControlPanel()
        {
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            ExecuteProcess(Path.Combine(systemPath, "control.exe"));
        }

        public static void OpenFileExplorer()
        {
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            ExecuteProcess(Path.Combine(systemPath, "explorer.exe"));
        }
    }
}
