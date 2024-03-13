using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using SystemMonitor.Logic.Utilities.DateTimes;
using SystemMonitor.Logic.Utilities.Drives;
using SystemMonitor.Tests.Fakes;

namespace SystemMonitor.Tests.Utilities
{
    internal class MonitorCommandTestServiceProvider : IServiceProvider
    {
        private readonly MonitorCommandServiceProvider monitorCommandServiceProvider;

        public MonitorCommandTestServiceProvider(
            CancellationToken cancellationToken, IReadOnlyCollection<Drive>? drives = null, DateTime? now = null)
        {
            // TODO: Study if this is producing concurrency errors at tests.
            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc =>
                {
                    if (drives is not null)
                    {
                        sc.AddScoped<IDrivesObtainer>(_ => new DrivesObtainerFake(drives));
                    }

                    if (now is not null)
                    {
                        sc.AddScoped<IDateTimeProvider>(_ => new DateTimeProviderFake(now.Value));
                    }
                };

            this.monitorCommandServiceProvider = new MonitorCommandServiceProvider(cancellationToken);
        }

        public object? GetService(Type serviceType)
        {
            return this.monitorCommandServiceProvider.GetService(serviceType);
        }
    }
}