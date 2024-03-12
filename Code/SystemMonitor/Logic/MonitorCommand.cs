using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities.Drives;

namespace SystemMonitor.Logic
{
    internal class MonitorCommand(
        IDrivesObtainer drivesObtainer, DirectoriesMonitor directoriesMonitor, OutputDirectory outputDirectory)
            : IMonitorCommand
    {
        public Task ExecuteAsync(string? directory)
        {
            if (directory is null)
            {
                return this.MonitorAllDrivesAsync();
            }

            return directoriesMonitor.MonitorAsync(
                directory, baseOutputDirectory: outputDirectory.Path, outputDirectory: outputDirectory.Path);
        }

        private Task MonitorAllDrivesAsync()
        {
            List<Task> tasks = [];

            string generalAllFileChangesFile = Path.Combine(outputDirectory.Path, OutputWriter.AllFileChangesFileName);

            string generalEventsFile = Path.Combine(outputDirectory.Path, OutputWriter.EventsFileName);

            foreach (Drive drive in drivesObtainer.GetDrives())
            {
                string driveOutputDirectory = Path.Combine(outputDirectory.Path, drive.VolumeLabel);

                Task task = directoriesMonitor.MonitorAsync(
                    drive.FullPath,
                    baseOutputDirectory: outputDirectory.Path,
                    driveOutputDirectory,
                    generalAllFileChangesFile,
                    generalEventsFile);

                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }
    }
}