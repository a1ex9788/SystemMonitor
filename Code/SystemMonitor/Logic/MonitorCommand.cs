using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities;

namespace SystemMonitor.Logic
{
    public class MonitorCommand(DirectoriesMonitor directoriesMonitor) : IMonitorCommand
    {
        public Task ExecuteAsync()
        {
            return directoriesMonitor.MonitorAsync(@"C:\");
        }
    }
}