using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic.Tests.Utilities;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.TestsUtilities;

namespace SystemMonitor.Logic.Tests.UnitTests
{
    [TestClass]
    public class MonitorCommandTests
    {
        [TestMethod]
        public async Task ExecuteAsync_DirectoryNotSpecified_MonitorAllDrives()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token);
            IMonitorCommand monitorCommand = serviceProvider
                .GetRequiredService<IMonitorCommand>();

            // Act.
            Task task = monitorCommand.ExecuteAsync(directory: null);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter, expectedCreatedFiles: [filePath]);

            foreach (string drive in DrivesObtainer.GetDrives())
            {
                stringWriter.ToString().Should().Contain($"Monitoring directory '{drive}'...");
            }

            cancellationTokenSource.Cancel();
            await task;
        }

        [TestMethod]
        public async Task ExecuteAsync_DirectorySpecified_DirectoryMonitored()
        {
            // Arrange.
            string parentDirectory = TempPathsObtainer.GetTempDirectory();
            string testDirectory = TempPathsObtainer.GetTempDirectory(parentDirectory);

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token);
            IMonitorCommand monitorCommand = serviceProvider
                .GetRequiredService<IMonitorCommand>();

            // Act.
            Task task = monitorCommand.ExecuteAsync(testDirectory);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string fileInParentPath = TempPathsObtainer.GetTempFile(parentDirectory);
            await File.Create(fileInParentPath).DisposeAsync();

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedNotCreatedFiles: [fileInParentPath],
                expectedCreatedFiles: [filePath]);

            stringWriter.ToString().Should().Contain($"Monitoring directory '{testDirectory}'...");

            cancellationTokenSource.Cancel();
            await task;
        }
    }
}