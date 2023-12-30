using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using SystemMonitor.Logic.Utilities.DateTimes;
using SystemMonitor.TestUtilities;

namespace SystemMonitor.Logic.Tests.Utilities
{
    internal class MonitorCommandTestServiceProvider : IServiceProvider
    {
        private readonly ServiceProvider serviceProvider;

        internal MonitorCommandTestServiceProvider(
            CancellationToken cancellationToken, DateTime? now = null)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(typeof(CancellationToken), cancellationToken);

            ConfigureServices(services);

            if (now is not null)
            {
                services.AddScoped<IDateTimeProvider>(_ => new DateTimeProviderFake(now.Value));
            }

            this.serviceProvider = services.BuildServiceProvider();
        }

        public object? GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSystemMonitorLogic();
        }
    }
}