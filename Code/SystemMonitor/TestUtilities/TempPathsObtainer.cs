using System;
using System.IO;

namespace SystemMonitor.TestsUtilities
{
    public class TempPathsObtainer
    {
        public static string GetTempDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }

        public static string GetTempFile(string directory)
        {
            return Path.Combine(directory, Guid.NewGuid().ToString());
        }

        public static string GetTempFile()
        {
            return Path.GetTempFileName();
        }
    }
}