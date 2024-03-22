using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using SystemMonitor.Logic.Drives;
using SystemMonitor.Logic.DateTimes;

namespace SystemMonitor.Logic
{
    internal class MonitorCommand : IMonitorCommand
    {
        private readonly IDrivesObtainer drivesObtainer;
        private readonly DirectoriesMonitor directoriesMonitor;

        private readonly string outputDirectory;

        public MonitorCommand(
            IDateTimeProvider dateTimeProvider,
            IDrivesObtainer drivesObtainer,
            IFileSystem fileSystem,
            DirectoriesMonitor directoriesMonitor)
        {
            this.drivesObtainer = drivesObtainer;
            this.directoriesMonitor = directoriesMonitor;

            string formattedData = dateTimeProvider.GetCurrentDateTime().ToDirectoryName();
            this.outputDirectory = Path.Combine(fileSystem.Directory.GetCurrentDirectory(), formattedData);
        }

        public Task ExecuteAsync(string? directory)
        {
            if (directory is null)
            {
                return this.MonitorAllDrivesAsync();
            }

            return this.directoriesMonitor.MonitorAsync(directory, new OutputFilesInfo(this.outputDirectory));
        }

        private Task MonitorAllDrivesAsync()
        {
            List<Task> tasks = [];

            foreach (Drive drive in this.drivesObtainer.GetDrives())
            {
                string driveOutputDirectory = Path.Combine(this.outputDirectory, drive.VolumeLabel);

                Task task = this.directoriesMonitor.MonitorAsync(
                    drive.FullPath,
                    new OutputFilesInfo(
                        outputDirectoryFullPath: driveOutputDirectory,
                        toolOutputDirectoryFullPath: this.outputDirectory));

                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }
    }
}