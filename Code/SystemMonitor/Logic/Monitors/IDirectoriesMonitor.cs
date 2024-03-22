using System.Threading.Tasks;

namespace SystemMonitor.Logic.Monitors
{
    internal interface IDirectoriesMonitor
    {
        Task MonitorAsync(string directory, OutputFilesInfo outputFilesInfo);
    }
}