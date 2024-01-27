using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities;

namespace SystemMonitor.Logic
{
    public class MonitorCommand(
        DirectoriesMonitor directoriesMonitor, OutputDirectory outputDirectory)
            : IMonitorCommand
    {
        public Task ExecuteAsync(string? directory)
        {
            if (directory is null)
            {
                return this.MonitorAllDrives();
            }

            return directoriesMonitor.MonitorAsync(
                directory, baseOutputDirectory: outputDirectory.Path, outputDirectory.Path);
        }

        private Task MonitorAllDrives()
        {
            List<Task> tasks = [];

            string generalAllFileChangesFile = Path.Combine(
                outputDirectory.Path, OutputWriter.AllFileChangesFileName);

            string generalEventsFile = Path.Combine(
                outputDirectory.Path, OutputWriter.EventsFileName);

            foreach (DriveInfo driveInfo in DrivesObtainer.GetDrives())
            {
                string driveOutputDirectory = Path.Combine(
                    outputDirectory.Path, driveInfo.VolumeLabel);

                Task task = directoriesMonitor.MonitorAsync(
                    driveInfo.RootDirectory.FullName,
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