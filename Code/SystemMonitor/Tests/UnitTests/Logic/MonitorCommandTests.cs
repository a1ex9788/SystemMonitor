using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.Logic.Utilities.DateTimes;
using SystemMonitor.Tests.Utilities;
using SystemMonitor.Tests.Utilities.Logic;

namespace SystemMonitor.Tests.UnitTests.Logic
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
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, now);
            IMonitorCommand monitorCommand = serviceProvider.GetRequiredService<IMonitorCommand>();

            // Act.
            Task task = monitorCommand.ExecuteAsync(directory: null);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(stringWriter, expectedCreatedFiles: [filePath]);

            IReadOnlyCollection<DriveInfo> drives = DrivesObtainer.GetDrives();

            foreach (string drive in drives.Select(di => di.RootDirectory.FullName))
            {
                stringWriter.ToString().Should().Contain($"Monitoring directory '{drive}'...");
            }

            cancellationTokenSource.Cancel();
            await task;

            string outputDirectory = now.ToDirectoryName();
            string expectedContent = $"{filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckAllFileChangesFileAsync(
                outputDirectory, expectedContent, exactContent: false);

            expectedContent = $"[{now}] Created: {filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(outputDirectory, expectedContent, exactContent: false);

            string fileDrive = new FileInfo(filePath).Directory!.Root.FullName;
            fileDrive = drives.First(di => di.RootDirectory.FullName == fileDrive).VolumeLabel;
            string fileDriveOutputDirectory = Path.Combine(outputDirectory, fileDrive);
            expectedContent = $"[{now}] Created: {filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(
                fileDriveOutputDirectory, expectedContent, exactContent: false);
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
            IMonitorCommand monitorCommand = serviceProvider.GetRequiredService<IMonitorCommand>();

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

        [TestMethod]
        public async Task ExecuteAsync_DirectoryNotSpecified_OutputFileChangesAreNotAnalysed()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token);
            IMonitorCommand monitorCommand = serviceProvider.GetRequiredService<IMonitorCommand>();

            // Act.
            Task task = monitorCommand.ExecuteAsync(directory: null);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);
            await File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(stringWriter, expectedCreatedFiles: [filePath]);

            IReadOnlyCollection<DriveInfo> drives = DrivesObtainer.GetDrives();

            foreach (string drive in drives.Select(di => di.RootDirectory.FullName))
            {
                stringWriter.ToString().Should().Contain($"Monitoring directory '{drive}'...");
            }

            cancellationTokenSource.Cancel();
            await task;

            stringWriter.ToString().Should().NotContain("AllFileChanges.txt");
            stringWriter.ToString().Should().NotContain("Events.txt");
        }
    }
}