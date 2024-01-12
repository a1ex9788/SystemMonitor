using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic.Tests.Utilities;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.TestUtilities;

namespace SystemMonitor.Logic.Tests.UnitTests.Utilities
{
    [TestClass]
    public class DirectoriesMonitorTests
    {
        [TestMethod]
        public async Task MonitorAsync_FileChanged_PrintsAndSavesFileName()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string outputDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, now);
            DirectoriesMonitor directoriesMonitor = serviceProvider
                .GetRequiredService<DirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, outputDirectory);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();
            await File.WriteAllTextAsync(filePath, string.Empty);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedChangedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            string expectedContent =
                $"[{now}] Created: {filePath}{Environment.NewLine}" +
                $"[{now}] Changed: {filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [filePath];
            await OutputFilesChecker.CheckChangesFile(
                outputDirectory, "ChangedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_FileCreated_PrintsAndSavesFileName()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string outputDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, now);
            DirectoriesMonitor directoriesMonitor = serviceProvider
                .GetRequiredService<DirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, outputDirectory);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedCreatedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            string expectedContent = $"[{now}] Created: {filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [filePath];
            await OutputFilesChecker.CheckChangesFile(
                outputDirectory, "CreatedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_FileDeleted_PrintsAndSavesFileName()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string outputDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, now);
            DirectoriesMonitor directoriesMonitor = serviceProvider
                .GetRequiredService<DirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, outputDirectory);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();
            File.Delete(filePath);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedDeletedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            string expectedContent =
                $"[{now}] Created: {filePath}{Environment.NewLine}" +
                $"[{now}] Deleted: {filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [filePath];
            await OutputFilesChecker.CheckChangesFile(
                outputDirectory, "DeletedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_FileRenamed_PrintsAndSavesFileName()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string outputDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, now);
            DirectoriesMonitor directoriesMonitor = serviceProvider
                .GetRequiredService<DirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, outputDirectory);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string oldFilePath = TempPathsObtainer.GetTempFile(testDirectory);
            string newFilePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(oldFilePath).DisposeAsync();
            File.Move(oldFilePath, newFilePath);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedRenamedFiles: [(oldFilePath, newFilePath)]);

            cancellationTokenSource.Cancel();
            await task;

            string expectedContent =
                $"[{now}] Created: {oldFilePath}{Environment.NewLine}" +
                $"[{now}] Renamed: {oldFilePath} to {newFilePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [$"{oldFilePath} -> {newFilePath}"];
            await OutputFilesChecker.CheckChangesFile(
                outputDirectory, "RenamedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_MonitoringCurrentDirectory_OutputFileChangesAreNotAnalysed()
        {
            // Arrange.
            string testDirectory = Directory.GetCurrentDirectory();
            string outputDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, now);
            DirectoriesMonitor directoriesMonitor = serviceProvider
                .GetRequiredService<DirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, outputDirectory);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();
            await File.WriteAllTextAsync(filePath, string.Empty);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedChangedFiles: [filePath],
                expectedCreatedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            stringWriter.ToString().Should().NotContain("Events.txt");

            string expectedContent =
                $"[{now}] Created: {filePath}{Environment.NewLine}" +
                $"[{now}] Changed: {filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(outputDirectory, expectedContent);
        }
    }
}