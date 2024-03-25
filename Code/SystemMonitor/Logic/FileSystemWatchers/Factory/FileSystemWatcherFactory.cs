namespace SystemMonitor.Logic.FileSystemWatchers.Factory
{
    internal class FileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        public IFileSystemWatcher Create(string directoryPath)
        {
            return new FileSystemWatcherWrapper(directoryPath);
        }
    }
}