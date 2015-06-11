using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using RemoteAppLauncher.Data.Models;
using File = System.IO.File;

namespace RemoteAppLauncher.Infrastructure.Utilities
{
    internal static class PathUtility
    {
        private static string[] AllowedExtensions = new string[3] { ".exe", ".lnk", ".appref-ms" };
        private static string _systemPath;
        private static string _controlPanelPath;
        private static string _fileExplorerPath;

        

        public static string SystemPath 
        {
            get { return _systemPath ?? (_systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System)); }
        }

        public static string ControlPanelPath
        {
            get { return _controlPanelPath ?? (_controlPanelPath = Path.Combine(SystemPath, "control.exe")); }
        }

        public static string FileExplorerPath
        {
            get
            {
                string path = SystemPath;
                if (Environment.Is64BitOperatingSystem)
                    path = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);

                return _fileExplorerPath ?? (_fileExplorerPath = Path.Combine(path, "explorer.exe"));
            }
        }

        public static bool IsPathDirectory(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            FileAttributes attributes = File.GetAttributes(path);
            return (attributes & FileAttributes.Directory) != 0;
        }

        public static IEnumerable<DirectoryEntry> GetDistinctDirectoryEntries(params string[] paths)
        {
            List<DirectoryEntry> entries = new List<DirectoryEntry>();

            foreach (var path in paths)
            {
                var temp = GetDirectoryEntries(path);
                foreach (var directoryEntry in temp)
                {
                    var existingEntry = entries.FirstOrDefault(x => x.Name.Equals(directoryEntry.Name, StringComparison.InvariantCultureIgnoreCase));

                    if (existingEntry != null)
                    {
                        if (existingEntry.IsDirectory == false)
                            continue;

                        if (existingEntry.Paths.Contains(directoryEntry.Paths[0]))
                            continue;

                        existingEntry.Paths.Add(directoryEntry.Paths[0]);
                        continue;
                    }

                    entries.Add(directoryEntry);
                }
            }

            return entries;
        }

        public static IEnumerable<DirectoryEntry> GetDirectoryEntries(string path, string parentDirectory = null)
        {
            if (string.IsNullOrEmpty(path) || !IsPathDirectory(path))
                throw new ArgumentException("path");

            string[] entries = Directory.GetDirectories(path);
            if (entries.Length > 0)
            {
                foreach (var entry in entries)
                {
                    var dirEntry = CreateEntry(entry, parentDirectory, true);
                    if(dirEntry.Name.Equals("desktop", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    yield return dirEntry;
                }
            }

            entries = Directory.GetFiles(path);
            if (entries.Length > 0)
            {
                foreach (var entry in entries)
                {
                    var dirEntry = CreateEntry(entry, parentDirectory, IsPathDirectory(entry));
                    string fileExtension = Path.GetExtension(dirEntry.Paths[0]);
                    if (dirEntry.Name.Equals("desktop", StringComparison.InvariantCultureIgnoreCase)
                        || (dirEntry.IsDirectory == false && !AllowedExtensions.Any(x => x.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))))
                        continue;

                    yield return dirEntry;
                }
            }
        }
        
        public static IEnumerable<DirectoryEntry> GetAllFileEntries()
        {
            string commonStartMenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string userStartMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);

            commonStartMenu = Path.Combine(commonStartMenu, "Programs");
            userStartMenu = Path.Combine(userStartMenu, "Programs");

            var toReturn = new List<DirectoryEntry>();

            WalkTree(commonStartMenu, toReturn);
            WalkTree(userStartMenu, toReturn);

            toReturn.Add(CreateEntry(ControlPanelPath, "_", false, "Control Panel"));
            toReturn.Add(CreateEntry(FileExplorerPath, "_", false, "My Computer"));

            return toReturn;
        }

        private static void WalkTree(string path, ICollection<DirectoryEntry> foundFiles, string parentDirectory = null)
        {
            if (string.IsNullOrEmpty(parentDirectory) || parentDirectory.Equals("programs", StringComparison.InvariantCultureIgnoreCase))
                parentDirectory = Path.GetFileName(path);

            var entries = GetDirectoryEntries(path, parentDirectory);
            foreach (var directoryEntry in entries)
            {
                if (!directoryEntry.IsDirectory)
                {
                    if (foundFiles.All(x => x.Paths[0] != directoryEntry.Paths[0]))
                    {
                        if (directoryEntry.ParentDirectory.Equals("programs", StringComparison.InvariantCultureIgnoreCase))
                            directoryEntry.ParentDirectory = "_";

                        foundFiles.Add(directoryEntry);
                    }

                    continue;
                }

                WalkTree(directoryEntry.Paths[0], foundFiles, parentDirectory);
            }
        }

        private static DirectoryEntry CreateEntry(string path, string parentDirectory, bool isDirectory, string entryName = null)
        {
            string name = isDirectory ? Path.GetFileName(parentDirectory) : Path.GetFileNameWithoutExtension(path);

            return new DirectoryEntry
                {
                    IsDirectory = isDirectory,
                    Name = entryName ?? name,
                    Paths = new List<string>{ path },
                    ParentDirectory = parentDirectory
                };
        }
    }
}
