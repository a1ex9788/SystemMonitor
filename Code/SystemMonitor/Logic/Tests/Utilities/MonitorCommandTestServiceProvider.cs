using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace SystemMonitor.Logic.Tests.Utilities
{
    internal class MonitorCommandTestServiceProvider : IServiceProvider
    {
        private readonly ServiceProvider serviceProvider;

        internal MonitorCommandTestServiceProvider(CancellationToken cancellationToken)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(typeof(CancellationToken), cancellationToken);

            ConfigureServices(services);

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