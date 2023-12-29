using System.Collections.Generic;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities;

namespace SystemMonitor.Logic
{
    public class MonitorCommand(DirectoriesMonitor directoriesMonitor) : IMonitorCommand
    {
        public Task ExecuteAsync()
        {
            List<Task> tasks = [];

            foreach (string drive in DrivesObtainer.GetDrives())
            {
                tasks.Add(directoriesMonitor.MonitorAsync(drive));
            }

            return Task.WhenAll(tasks);
        }
    }
}