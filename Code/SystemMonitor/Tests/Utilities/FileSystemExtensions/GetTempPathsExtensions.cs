using SystemMonitor.Tests.Utilities;

namespace System.IO.Abstractions
{
    internal static class GetTempPathsExtensions
    {
        public static string GetTempDirectory(
            this IFileSystem fileSystem, string parentDirectory, bool createDirectory = false)
        {
            return TempPathsObtainer.GetTempDirectory(parentDirectory, fileSystem, createDirectory);
        }

        public static string GetTempDirectory(this IFileSystem fileSystem, bool createDirectory = false)
        {
            return TempPathsObtainer.GetTempDirectory(fileSystem, createDirectory: createDirectory);
        }

        public static string GetTempFile(this IFileSystem _, string parentDirectory)
        {
            return Path.Combine(parentDirectory, Guid.NewGuid().ToString());
        }
    }
}