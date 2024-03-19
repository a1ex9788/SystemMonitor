using System;
using System.IO;
using System.IO.Abstractions;

namespace SystemMonitor.Tests.Utilities
{
    internal static class TempPathsObtainer
    {
        public static string GetTempDirectory(
            string parentDirectory, IFileSystem? fileSystem = null, bool createDirectory = false)
        {
            string tempDirectory = Path.Combine(parentDirectory, Guid.NewGuid().ToString());

            if (createDirectory)
            {
                if (fileSystem is null)
                {
                    Directory.CreateDirectory(tempDirectory);
                }
                else
                {
                    fileSystem.Directory.CreateDirectory(tempDirectory);
                }
            }

            return tempDirectory;
        }

        public static string GetTempDirectory(IFileSystem? fileSystem = null, bool createDirectory = false)
        {
            string tempPath = fileSystem is null ? Path.GetTempPath() : fileSystem.Path.GetTempPath();

            return GetTempDirectory(tempPath, fileSystem, createDirectory);
        }
    }
}