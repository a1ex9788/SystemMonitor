using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic
{
    public static class LogicServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemMonitorLogic(this IServiceCollection services)
        {
            services.AddScoped<IMonitorCommand, MonitorCommand>();

            services.AddScoped<DirectoriesMonitor>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}