using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Tests.Utilities.Logic
{
    internal class MonitorCommandTestServiceProvider : IServiceProvider
    {
        private readonly MonitorCommandServiceProvider monitorCommandServiceProvider;

        internal MonitorCommandTestServiceProvider(
            CancellationToken cancellationToken, DateTime? now = null)
        {
            if (now is not null)
            {
                MonitorCommandServiceProvider.ExtraRegistrationsAction =
                    sc =>
                    {
                        sc.AddScoped<IDateTimeProvider>(_ => new DateTimeProviderFake(now.Value));
                    };
            }

            this.monitorCommandServiceProvider = new MonitorCommandServiceProvider(
                cancellationToken);
        }

        public object? GetService(Type serviceType)
        {
            return this.monitorCommandServiceProvider.GetService(serviceType);
        }
    }
}