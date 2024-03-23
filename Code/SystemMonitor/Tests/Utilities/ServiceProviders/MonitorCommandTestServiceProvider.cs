using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using SystemMonitor.Logic.DateTimes;
using SystemMonitor.Logic.Drives;
using SystemMonitor.Tests.Fakes;

namespace SystemMonitor.Tests.Utilities.ServiceProviders
{
    internal class MonitorCommandTestServiceProvider : IServiceProvider
    {
        private readonly MonitorCommandServiceProvider monitorCommandServiceProvider;

        public MonitorCommandTestServiceProvider(TestServiceProviderOptions? testServiceProviderOptions = null)
        {
            if (testServiceProviderOptions is not null)
            {
                // TODO: Study if this is producing concurrency errors at tests.
                MonitorCommandServiceProvider.ExtraRegistrationsAction =
                    sc =>
                    {
                        if (testServiceProviderOptions.DirectoriesMonitor is not null)
                        {
                            sc.AddScoped(_ => testServiceProviderOptions.DirectoriesMonitor);
                        }

                        if (testServiceProviderOptions.Drives is not null)
                        {
                            sc.AddScoped<IDrivesObtainer>(_ => new DrivesObtainerFake(testServiceProviderOptions.Drives));
                        }

                        if (testServiceProviderOptions.FileSystem is not null)
                        {
                            sc.AddScoped<IFileSystem>(_ => testServiceProviderOptions.FileSystem);

                            sc.AddScoped<IFileSystemWatcherFactory>(
                                _ => new MockFileSystemWatcherFactory(testServiceProviderOptions.FileSystem));
                        }

                        if (testServiceProviderOptions.Now is not null)
                        {
                            sc.AddScoped<IDateTimeProvider>(
                                _ => new DateTimeProviderFake(testServiceProviderOptions.Now.Value));
                        }

                        if (testServiceProviderOptions.OutputWriterFactory is not null)
                        {
                            sc.AddScoped(_ => testServiceProviderOptions.OutputWriterFactory);
                        }
                    };
            }

            CancellationToken cancellationToken = testServiceProviderOptions?.CancellationToken
                ?? CancellationToken.None;

            this.monitorCommandServiceProvider = new MonitorCommandServiceProvider(cancellationToken);
        }

        public object? GetService(Type serviceType)
        {
            return this.monitorCommandServiceProvider.GetService(serviceType);
        }
    }
}