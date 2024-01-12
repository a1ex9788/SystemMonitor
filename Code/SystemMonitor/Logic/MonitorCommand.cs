using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic
{
    public class MonitorCommand(
        DirectoriesMonitor directoriesMonitor, IDateTimeProvider dateTimeProvider)
            : IMonitorCommand
    {
        public Task ExecuteAsync(string? directory)
        {
            string formattedData = dateTimeProvider.GetCurrentDateTime().ToDirectoryName();
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), formattedData);

            if (directory is null)
            {
                List<Task> tasks = [];

                foreach (string drive in DrivesObtainer.GetDrives())
                {
                    tasks.Add(directoriesMonitor.MonitorAsync(drive, outputDirectory));
                }

                return Task.WhenAll(tasks);
            }

            return directoriesMonitor.MonitorAsync(directory, outputDirectory);
        }
    }
}