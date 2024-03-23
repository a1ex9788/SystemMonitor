using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using SystemMonitor.Logic.Drives;
using SystemMonitor.Logic.Monitors;
using SystemMonitor.Logic.Output.Factory;

namespace SystemMonitor.Tests.Utilities.ServiceProviders
{
    internal class TestServiceProviderOptions
    {
        public CancellationToken? CancellationToken { get; set; }

        public IDirectoriesMonitor? DirectoriesMonitor { get; set; }

        public IReadOnlyCollection<Drive>? Drives { get; set; }

        public MockFileSystem? FileSystem { get; set; }

        public DateTime? Now { get; set; }

        public IOutputWriterFactory? OutputWriterFactory { get; set; }
    }
}