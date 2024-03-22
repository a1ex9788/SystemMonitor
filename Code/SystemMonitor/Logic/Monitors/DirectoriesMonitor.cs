using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Exceptions;
using SystemMonitor.Logic.DateTimes;
using SystemMonitor.Logic.Output;
using SystemMonitor.Logic.Output.Factory;

namespace SystemMonitor.Logic.Monitors
{
    internal class DirectoriesMonitor(
        IDateTimeProvider dateTimeProvider,
        IFileSystemWatcherFactory fileSystemWatcherFactory,
        IFileSystem fileSystem,
        IOutputWriterFactory outputWriterFactory,
        CancellationToken cancellationToken)
            : IDirectoriesMonitor
    {
        private readonly IDirectory directory = fileSystem.Directory;
        private readonly IFile file = fileSystem.File;

        public async Task MonitorAsync(string directory, OutputFilesInfo outputFilesInfo)
        {
            if (!this.directory.Exists(directory))
            {
                throw new NotExistingDirectoryException(directory);
            }

            Console.WriteLine("Monitoring directory '{0}'...", directory);

            using IFileSystemWatcher fileSystemWatcher = fileSystemWatcherFactory.New(directory);

            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;

            IOutputWriter outputWriter = outputWriterFactory.CreateOutputWriter(
                dateTimeProvider, this.directory, this.file, outputFilesInfo);

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

        private static FileSystemEventHandler OnChanged(IOutputWriter outputWriter)
        {
            return (object sender, FileSystemEventArgs e) =>
            {
                outputWriter.WriteChangedFile(e.FullPath);
            };
        }

        private static FileSystemEventHandler OnCreated(IOutputWriter outputWriter)
        {
            return (object sender, FileSystemEventArgs e) =>
            {
                outputWriter.WriteCreatedFile(e.FullPath);
            };
        }

        private static FileSystemEventHandler OnDeleted(IOutputWriter outputWriter)
        {
            return (object sender, FileSystemEventArgs e) =>
            {
                outputWriter.WriteDeletedFile(e.FullPath);
            };
        }

        private static RenamedEventHandler OnRenamed(IOutputWriter outputWriter)
        {
            return (object sender, RenamedEventArgs e) =>
            {
                outputWriter.WriteRenamedFile(e.OldFullPath, e.FullPath);
            };
        }

        private static ErrorEventHandler OnError(IOutputWriter outputWriter)
        {
            return (object sender, ErrorEventArgs e) =>
            {
                outputWriter.WriteError(e.GetException().Message);
            };
        }
    }
}