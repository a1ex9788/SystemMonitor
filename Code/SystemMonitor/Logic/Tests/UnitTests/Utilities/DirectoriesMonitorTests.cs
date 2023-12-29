using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.TestsUtilities;

namespace SystemMonitor.Logic.Tests.UnitTests.Utilities
{
    [TestClass]
    public class DirectoriesMonitorTests
    {
        [TestMethod]
        public async Task MonitorAsync_FileChanged_PrintsFileName()
        {
            // Arrange.
            string testPath = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DirectoriesMonitor directoriesMonitor = new DirectoriesMonitor(
                cancellationTokenSource.Token);

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testPath);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testPath);
            await File.Create(filePath).DisposeAsync();
            await File.WriteAllTextAsync(filePath, string.Empty);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedChangedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;
        }

        [TestMethod]
        public async Task MonitorAsync_FileCreated_PrintsFileName()
        {
            // Arrange.
            string testPath = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DirectoriesMonitor directoriesMonitor = new DirectoriesMonitor(
                cancellationTokenSource.Token);

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testPath);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testPath);
            await File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedCreatedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;
        }

        [TestMethod]
        public async Task MonitorAsync_FileDeleted_PrintsFileName()
        {
            // Arrange.
            string testPath = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DirectoriesMonitor directoriesMonitor = new DirectoriesMonitor(
                cancellationTokenSource.Token);

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testPath);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testPath);
            await File.Create(filePath).DisposeAsync();
            File.Delete(filePath);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedDeletedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;
        }

        [TestMethod]
        public async Task MonitorAsync_FileRenamed_PrintsFileName()
        {
            // Arrange.
            string testPath = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DirectoriesMonitor directoriesMonitor = new DirectoriesMonitor(
                cancellationTokenSource.Token);

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testPath);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string oldFilePath = TempPathsObtainer.GetTempFile(testPath);
            string newFilePath = TempPathsObtainer.GetTempFile(testPath);
            await File.Create(oldFilePath).DisposeAsync();
            File.Move(oldFilePath, newFilePath);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedRenamedFiles: [(oldFilePath, newFilePath)]);

            cancellationTokenSource.Cancel();
            await task;
        }
    }
}