using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic.Utilities
{
    public class DirectoriesMonitor(
        IDateTimeProvider dateTimeProvider, CancellationToken cancellationToken)
    {
        public async Task MonitorAsync(string directory)
        {
            Console.WriteLine("Monitoring directory '{0}'...", directory);

            using FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(directory);

            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;

            fileSystemWatcher.Changed += this.OnChanged;
            fileSystemWatcher.Created += this.OnCreated;
            fileSystemWatcher.Deleted += this.OnDeleted;
            fileSystemWatcher.Renamed += this.OnRenamed;
            fileSystemWatcher.Error += this.OnError;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                // The command is cancelled this way.
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            this.PrintMessage($"Changed: {e.FullPath}");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            this.PrintMessage($"Created: {e.FullPath}");
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            this.PrintMessage($"Deleted: {e.FullPath}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            this.PrintMessage($"Renamed: {e.OldFullPath} to {e.FullPath}");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            this.PrintMessage($"Error: {e.GetException()}");
        }

        private void PrintMessage(string message)
        {
            Console.WriteLine($"[{dateTimeProvider.GetCurrentDateTime()}] {message}");
        }
    }
}