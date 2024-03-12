using System;
using System.IO;

namespace SystemMonitor.Tests.Utilities
{
    internal static class TempPathsObtainer
    {
        public static string GetTempDirectory(string parentDirectory)
        {
            string tempDirectory = Path.Combine(parentDirectory, Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }

        public static string GetTempDirectory()
        {
            return GetTempDirectory(Path.GetTempPath());
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