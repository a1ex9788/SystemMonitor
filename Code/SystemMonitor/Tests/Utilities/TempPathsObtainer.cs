using System;
using System.IO;

namespace SystemMonitor.Tests.Utilities
{
    internal static class TempPathsObtainer
    {
        internal static string GetTempDirectory(string parentDirectory)
        {
            string tempDirectory = Path.Combine(parentDirectory, Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }

        internal static string GetTempDirectory()
        {
            return GetTempDirectory(Path.GetTempPath());
        }

        internal static string GetTempFile(string parentDirectory)
        {
            return Path.Combine(parentDirectory, Guid.NewGuid().ToString());
        }

        internal static string GetTempFile()
        {
            return Path.GetTempFileName();
        }
    }
}