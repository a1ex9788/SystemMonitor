using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            string testPath = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            MonitorCommand monitorCommand = new MonitorCommand(
                new DirectoriesMonitor(cancellationTokenSource.Token));

            // Act.
            Task task = monitorCommand.ExecuteAsync();

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = Path.Combine(testPath, Guid.NewGuid().ToString());
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
    }
}