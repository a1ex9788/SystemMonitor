using System.Threading.Tasks;

namespace SystemMonitor.Logic
{
    public interface IMonitorCommand
    {
        Task ExecuteAsync();
    }
}