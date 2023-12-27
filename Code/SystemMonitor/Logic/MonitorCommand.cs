using System;
using System.Threading.Tasks;

namespace SystemMonitor.Logic
{
    public class MonitorCommand : IMonitorCommand
    {
        public Task ExecuteAsync()
        {
            Console.Write("Hola");

            return Task.CompletedTask;
        }
    }
}