namespace SystemMonitor.Logic.FileSystemWatchers.Factory
{
    internal interface IFileSystemWatcherFactory
    {
        IFileSystemWatcher Create(string directoryPath);
    }
}