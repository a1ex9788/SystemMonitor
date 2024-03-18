using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Exceptions;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic
{
    internal class DirectoriesMonitor(IDateTimeProvider dateTimeProvider, CancellationToken cancellationToken)
    {
        public async Task MonitorAsync(
            string directory,
            string baseOutputDirectory,
            string outputDirectory,
            string? generalAllFileChangesFile = null,
            string? generalEventsFile = null)
        {
            if (!Directory.Exists(directory))
            {
                throw new NotExistingDirectoryException(directory);
            }

            Console.WriteLine("Monitoring directory '{0}'...", directory);

            using FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(directory);

            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;

            OutputWriter outputWriter = new OutputWriter(
                baseOutputDirectory,
                outputDirectory,
                dateTimeProvider,
                generalAllFileChangesFile,
                generalEventsFile);

            fileSystemWatcher.Changed += OnChanged(outputWriter);
            fileSystemWatcher.Created += OnCreated(outputWriter);
            fileSystemWatcher.Deleted += OnDeleted(outputWriter);
            fileSystemWatcher.Renamed += OnRenamed(outputWriter);
            fileSystemWatcher.Error += OnError(outputWriter);

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

        private static FileSystemEventHandler OnChanged(OutputWriter outputWriter)
        {
            return (object sender, FileSystemEventArgs e) =>
            {
                outputWriter.WriteChangedFile(e.FullPath);
            };
        }

        private static FileSystemEventHandler OnCreated(OutputWriter outputWriter)
        {
            return (object sender, FileSystemEventArgs e) =>
            {
                outputWriter.WriteCreatedFile(e.FullPath);
            };
        }

        private static FileSystemEventHandler OnDeleted(OutputWriter outputWriter)
        {
            return (object sender, FileSystemEventArgs e) =>
            {
                outputWriter.WriteDeletedFile(e.FullPath);
            };
        }

        private static RenamedEventHandler OnRenamed(OutputWriter outputWriter)
        {
            return (object sender, RenamedEventArgs e) =>
            {
                outputWriter.WriteRenamedFile(e.OldFullPath, e.FullPath);
            };
        }

        private static ErrorEventHandler OnError(OutputWriter outputWriter)
        {
            return (object sender, ErrorEventArgs e) =>
            {
                outputWriter.WriteError(e.GetException().Message);
            };
        }
    }
}