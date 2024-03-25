using System.IO;

namespace SystemMonitor.Logic.FileSystemWatchers
{
    internal class FileSystemWatcherWrapper : IFileSystemWatcher
    {
        private readonly FileSystemWatcher fileSystemWatcher;

        public FileSystemWatcherWrapper(string directoryPath)
        {
            this.fileSystemWatcher = new FileSystemWatcher(directoryPath)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
            };

            this.fileSystemWatcher.Changed += this.Changed;
            this.fileSystemWatcher.Created += this.Created;
            this.fileSystemWatcher.Deleted += this.Deleted;
            this.fileSystemWatcher.Renamed += this.Renamed;
            this.fileSystemWatcher.Error += this.Error;
        }

        public event FileSystemEventHandler? Changed;

        public event FileSystemEventHandler? Created;

        public event FileSystemEventHandler? Deleted;

        public event RenamedEventHandler? Renamed;

        public event ErrorEventHandler? Error;

        public void Dispose()
        {
            this.fileSystemWatcher.Dispose();
        }
    }
}