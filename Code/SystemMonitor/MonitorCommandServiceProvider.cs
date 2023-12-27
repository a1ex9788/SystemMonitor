using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using SystemMonitor.Logic;

namespace SystemMonitor
{
    internal class MonitorCommandServiceProvider : IServiceProvider
    {
        private readonly ServiceProvider serviceProvider;

        // Hook for tests.
        public static Action<IServiceCollection>? ExtraRegistrationsAction;

        public MonitorCommandServiceProvider(CancellationToken cancellationToken)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(typeof(CancellationToken), cancellationToken);

            ConfigureServices(services);

            ExtraRegistrationsAction?.Invoke(services);

            this.serviceProvider = services.BuildServiceProvider();
        }

        public object? GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMonitorCommand, MonitorCommand>();
        }
    }
}