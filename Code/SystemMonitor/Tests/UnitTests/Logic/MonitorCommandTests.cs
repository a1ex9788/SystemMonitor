using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic;
using SystemMonitor.Logic.Drives;
using SystemMonitor.Logic.DateTimes;
using SystemMonitor.Tests.Utilities;

namespace SystemMonitor.Tests.UnitTests.Logic
{
    [TestClass]
    public class MonitorCommandTests
    {
        // TODO: Make not flaky.
        [TestMethod]
        public async Task ExecuteAsync_DirectoryNotSpecified_MonitorAllDrives()
        {
            // Arrange.
            FileSystem mockFileSystem = new FileSystem();

            string testDirectory = mockFileSystem.GetTempDirectory(createDirectory: true);

            Drive[] drives =
            [
                new Drive("C", @"C:\"),
                new Drive("D", @"D:\"),
            ];

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, drives, mockFileSystem, now);
            IMonitorCommand monitorCommand = serviceProvider.GetRequiredService<IMonitorCommand>();

            // Act.
            Task task = monitorCommand.ExecuteAsync(directory: null);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(stringWriter, expectedCreatedFiles: [filePath]);

            foreach (string drive in drives.Select(di => di.FullPath))
            {
                stringWriter.ToString().Should().Contain($"Monitoring directory '{drive}'...");
            }

            cancellationTokenSource.Cancel();
            await task;

            string outputDirectory = now.ToDirectoryName();
            string expectedContent = $"{filePath}{Environment.NewLine}";
            await mockFileSystem.CheckAllFileChangesFileAsync(outputDirectory, expectedContent, exactContent: false);

            expectedContent = $"[{now}] Created: {filePath}{Environment.NewLine}";
            await mockFileSystem.CheckEventsFileAsync(outputDirectory, expectedContent, exactContent: false);

            string fileDrive = new FileInfo(filePath).Directory!.Root.FullName;
            fileDrive = drives.First(di => di.FullPath == fileDrive).VolumeLabel;
            string fileDriveOutputDirectory = Path.Combine(outputDirectory, fileDrive);
            expectedContent = $"[{now}] Created: {filePath}{Environment.NewLine}";
            await mockFileSystem.CheckEventsFileAsync(fileDriveOutputDirectory, expectedContent, exactContent: false);
        }

        [TestMethod]
        public async Task ExecuteAsync_DirectorySpecified_DirectoryMonitored()
        {
            // Arrange.
            FileSystem mockFileSystem = new FileSystem();
            string parentDirectory = mockFileSystem.GetTempDirectory();
            string testDirectory = mockFileSystem.GetTempDirectory(parentDirectory, createDirectory: true);

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, fileSystem: mockFileSystem);
            IMonitorCommand monitorCommand = serviceProvider.GetRequiredService<IMonitorCommand>();

            // Act.
            Task task = monitorCommand.ExecuteAsync(testDirectory);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string fileInParentPath = mockFileSystem.GetTempFile(parentDirectory);
            await mockFileSystem.File.Create(fileInParentPath).DisposeAsync();

            string filePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(filePath).DisposeAsync();

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
            FileSystem mockFileSystem = new FileSystem();
            string testDirectory = mockFileSystem.GetTempDirectory(createDirectory: true);
            Drive[] drives = [new Drive("C", @"C:\")];

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                cancellationTokenSource.Token, drives, mockFileSystem);
            IMonitorCommand monitorCommand = serviceProvider.GetRequiredService<IMonitorCommand>();

            // Act.
            Task task = monitorCommand.ExecuteAsync(directory: null);

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(stringWriter, expectedCreatedFiles: [filePath]);

            foreach (string drive in drives.Select(di => di.FullPath))
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