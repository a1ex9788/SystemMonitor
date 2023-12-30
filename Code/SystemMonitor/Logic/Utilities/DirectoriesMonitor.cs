using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SystemMonitor.Logic.Utilities
{
    public class DirectoriesMonitor(OutputWriter outputWriter, CancellationToken cancellationToken)
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
            outputWriter.WriteChangedFile(e.FullPath);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            outputWriter.WriteCreatedFile(e.FullPath);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            outputWriter.WriteDeletedFile(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            outputWriter.WriteRenamedFile(e.OldFullPath, e.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            outputWriter.WriteError(e.GetException().Message);
        }
    }
}