using System.Threading.Tasks;

namespace SystemMonitor.Logic
{
    internal interface IMonitorCommand
    {
        Task ExecuteAsync(string? directory);
    }
}