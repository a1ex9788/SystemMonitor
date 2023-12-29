using System.Collections.Generic;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities;

namespace SystemMonitor.Logic
{
    public class MonitorCommand(DirectoriesMonitor directoriesMonitor) : IMonitorCommand
    {
        public Task ExecuteAsync(string? directory)
        {
            if (directory is null)
            {
                List<Task> tasks = [];

                foreach (string drive in DrivesObtainer.GetDrives())
                {
                    tasks.Add(directoriesMonitor.MonitorAsync(drive));
                }

                return Task.WhenAll(tasks);
            }

            return directoriesMonitor.MonitorAsync(directory);
        }
    }
}