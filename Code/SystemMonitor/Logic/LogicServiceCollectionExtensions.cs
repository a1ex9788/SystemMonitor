using SystemMonitor.Logic;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LogicServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemMonitorLogic(this IServiceCollection services)
        {
            services.AddScoped<IMonitorCommand, MonitorCommand>();

            services.AddScoped<DirectoriesMonitor>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<OutputWriter>();

            return services;
        }
    }
}