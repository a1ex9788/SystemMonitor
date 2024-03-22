using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;
using System.Threading;
using SystemMonitor.Logic;
using SystemMonitor.Logic.DateTimes;
using SystemMonitor.Logic.Drives;
using SystemMonitor.Logic.Monitors;
using SystemMonitor.Logic.Output.Factory;

namespace SystemMonitor
{
    internal class MonitorCommandServiceProvider : IServiceProvider
    {
        private readonly ServiceProvider serviceProvider;

        // Hook for tests.
        public static Action<IServiceCollection>? ExtraRegistrationsAction;

        public MonitorCommandServiceProvider(CancellationToken? cancellationToken = null)
        {
            IServiceCollection services = new ServiceCollection();

            if (cancellationToken is not null)
            {
                services.AddSingleton(typeof(CancellationToken), cancellationToken);
            }

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
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.AddScoped<IDrivesObtainer, DrivesObtainer>();

            services.AddScoped<IDirectoriesMonitor, DirectoriesMonitor>();

            services.AddScoped<IOutputWriterFactory, OutputWriterFactory>();

            services.AddScoped<IMonitorCommand, MonitorCommand>();

            services.AddScoped<IFileSystemWatcherFactory, FileSystemWatcherFactory>();
            services.AddSingleton<IFileSystem, FileSystem>();
        }
    }
}