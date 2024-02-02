using System.Threading.Tasks;

namespace SystemMonitor.Logic
{
    internal interface IMonitorCommand
    {
        internal Task ExecuteAsync(string? directory);
    }
}