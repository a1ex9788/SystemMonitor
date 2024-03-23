using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;
using SystemMonitor.Logic.Drives;

namespace SystemMonitor.Tests.Utilities.ServiceProviders
{
    internal class TestServiceProviderOptions
    {
        public CancellationToken? CancellationToken { get; set; }

        public IReadOnlyCollection<Drive>? Drives { get; set; }

        public IFileSystem? FileSystem { get; set; }

        public DateTime? Now { get; set; }
    }
}