using RemoteAppLauncher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAppLauncher.Infrastructure
{
    internal static class PathUtility
    {
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

        public static IEnumerable<DirectoryEntry> GetDirectoryEntries(string path)
        {
            if (string.IsNullOrEmpty(path) || !IsPathDirectory(path))
                throw new ArgumentException("path");

            string[] entries = Directory.GetDirectories(path);
            if (entries.Length > 0)
            {
                foreach (var entry in entries)
                {
                    var dirEntry = CreateEntry(entry, true);
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
                    var dirEntry = CreateEntry(entry, IsPathDirectory(entry));
                    if (dirEntry.Name.Equals("desktop", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    yield return dirEntry;
                }
            }
        }

        private static DirectoryEntry CreateEntry(string path, bool isDirectory)
        {
            string name = (isDirectory)
                              ? Path.GetFileName(path)
                              : Path.GetFileNameWithoutExtension(path);

            return new DirectoryEntry
                {
                    IsDirectory = isDirectory,
                    Name = name,
                    Paths = new List<string>{ path }
                };
        }
    }
}
