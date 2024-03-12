using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using SystemMonitor.Logic.Utilities.DateTimes;
using SystemMonitor.Tests.Fakes;

namespace SystemMonitor.Tests.Utilities
{
    internal class MonitorCommandTestServiceProvider : IServiceProvider
    {
        private readonly MonitorCommandServiceProvider monitorCommandServiceProvider;

        public MonitorCommandTestServiceProvider(CancellationToken cancellationToken, DateTime? now = null)
        {
            if (now is not null)
            {
                MonitorCommandServiceProvider.ExtraRegistrationsAction =
                    sc =>
                    {
                        sc.AddScoped<IDateTimeProvider>(_ => new DateTimeProviderFake(now.Value));
                    };
            }

            this.monitorCommandServiceProvider = new MonitorCommandServiceProvider(cancellationToken);
        }

        public object? GetService(Type serviceType)
        {
            return this.monitorCommandServiceProvider.GetService(serviceType);
        }
    }
}