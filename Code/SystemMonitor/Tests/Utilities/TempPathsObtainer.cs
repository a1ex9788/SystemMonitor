using System;
using System.IO;

namespace SystemMonitor.Tests.Utilities
{
    internal static class TempPathsObtainer
    {
        public static string GetTempDirectory(string parentDirectory, bool createDirectory = true)
        {
            string tempDirectory = Path.Combine(parentDirectory, Guid.NewGuid().ToString());

            if (createDirectory)
            {
                Directory.CreateDirectory(tempDirectory);
            }

            return tempDirectory;
        }

        public static string GetTempDirectory(bool createDirectory = true)
        {
            return GetTempDirectory(Path.GetTempPath(), createDirectory);
        }

        public static string GetTempFile(string parentDirectory)
        {
            return Path.Combine(parentDirectory, Guid.NewGuid().ToString());
        }

        public static string GetTempFile()
        {
            return Path.GetTempFileName();
        }
    }
}