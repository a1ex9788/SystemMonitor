using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;
using SystemMonitor.Logic.Drives;
using SystemMonitor.Logic.DateTimes;
using SystemMonitor.Tests.Fakes;

namespace SystemMonitor.Tests.Utilities
{
    internal class MonitorCommandTestServiceProvider : IServiceProvider
    {
        private readonly MonitorCommandServiceProvider monitorCommandServiceProvider;

        public MonitorCommandTestServiceProvider(
            CancellationToken? cancellationToken = null,
            IReadOnlyCollection<Drive>? drives = null,
            IFileSystem? fileSystem = null,
            DateTime? now = null)
        {
            cancellationToken ??= CancellationToken.None;

            // TODO: Study if this is producing concurrency errors at tests.
            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc =>
                {
                    if (drives is not null)
                    {
                        sc.AddScoped<IDrivesObtainer>(_ => new DrivesObtainerFake(drives));
                    }

                    if (fileSystem is not null)
                    {
                        sc.AddScoped(_ => fileSystem);
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