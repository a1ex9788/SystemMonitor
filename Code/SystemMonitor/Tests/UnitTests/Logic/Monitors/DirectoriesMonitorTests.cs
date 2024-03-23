using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Exceptions;
using SystemMonitor.Logic.Monitors;
using SystemMonitor.Tests.Utilities;
using SystemMonitor.Tests.Utilities.ServiceProviders;

namespace SystemMonitor.Tests.UnitTests.Logic.Monitors
{
    [TestClass]
    public class DirectoriesMonitorTests
    {
        [TestMethod]
        public async Task MonitorAsync_NotExistingDirectory_ThrowsException()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string outputDirectory = TempPathsObtainer.GetTempDirectory();

            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider();
            IDirectoriesMonitor directoriesMonitor = serviceProvider.GetRequiredService<IDirectoriesMonitor>();

            // Act.
            Func<Task> action =
                () => directoriesMonitor.MonitorAsync(testDirectory, new OutputFilesInfo(outputDirectory));

            // Assert.
            await action.Should().ThrowAsync<NotExistingDirectoryException>();
        }

        [TestMethod]
        public async Task MonitorAsync_FileChanged_PrintsAndSavesFileName()
        {
            // Arrange.
            FileSystem mockFileSystem = new FileSystem();

            string testDirectory = mockFileSystem.GetTempDirectory(createDirectory: true);
            string outputDirectory = mockFileSystem.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                new TestServiceProviderOptions
                {
                    CancellationToken = cancellationTokenSource.Token,
                    FileSystem = mockFileSystem,
                    Now = now,
                });
            IDirectoriesMonitor directoriesMonitor = serviceProvider.GetRequiredService<IDirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, new OutputFilesInfo(outputDirectory));

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(filePath).DisposeAsync();
            await mockFileSystem.File.WriteAllTextAsync(filePath, string.Empty);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(stringWriter, expectedChangedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            string expectedContent = $"{filePath}{Environment.NewLine}";
            await mockFileSystem.CheckAllFileChangesFileAsync(outputDirectory, expectedContent);

            expectedContent =
                $"[{now}] Created: {filePath}{Environment.NewLine}" +
                $"[{now}] Changed: {filePath}{Environment.NewLine}";
            await mockFileSystem.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [filePath];
            await mockFileSystem.CheckChangesFile(outputDirectory, "ChangedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_FileCreated_PrintsAndSavesFileName()
        {
            // Arrange.
            FileSystem mockFileSystem = new FileSystem();

            string testDirectory = mockFileSystem.GetTempDirectory(createDirectory: true);
            string outputDirectory = mockFileSystem.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                new TestServiceProviderOptions
                {
                    CancellationToken = cancellationTokenSource.Token,
                    FileSystem = mockFileSystem,
                    Now = now,
                });
            IDirectoriesMonitor directoriesMonitor = serviceProvider.GetRequiredService<IDirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, new OutputFilesInfo(outputDirectory));

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(filePath).DisposeAsync();

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(stringWriter, expectedCreatedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            await Task.Delay(10);

            string expectedContent = $"{filePath}{Environment.NewLine}";
            await mockFileSystem.CheckAllFileChangesFileAsync(outputDirectory, expectedContent);

            expectedContent = $"[{now}] Created: {filePath}{Environment.NewLine}";
            await mockFileSystem.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [filePath];
            await mockFileSystem.CheckChangesFile(outputDirectory, "CreatedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_FileDeleted_PrintsAndSavesFileName()
        {
            // Arrange.
            FileSystem mockFileSystem = new FileSystem();

            string testDirectory = mockFileSystem.GetTempDirectory(createDirectory: true);
            string outputDirectory = mockFileSystem.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                new TestServiceProviderOptions
                {
                    CancellationToken = cancellationTokenSource.Token,
                    FileSystem = mockFileSystem,
                    Now = now,
                });
            IDirectoriesMonitor directoriesMonitor = serviceProvider.GetRequiredService<IDirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, new OutputFilesInfo(outputDirectory));

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(filePath).DisposeAsync();
            mockFileSystem.File.Delete(filePath);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter, expectedDeletedFiles: [filePath]);
            cancellationTokenSource.Cancel();
            await task;

            string expectedContent = $"{filePath}{Environment.NewLine}";
            await mockFileSystem.CheckAllFileChangesFileAsync(outputDirectory, expectedContent);

            expectedContent =
                $"[{now}] Created: {filePath}{Environment.NewLine}" +
                $"[{now}] Deleted: {filePath}{Environment.NewLine}";
            await mockFileSystem.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [filePath];
            await mockFileSystem.CheckChangesFile(outputDirectory, "DeletedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_FileRenamed_PrintsAndSavesFileName()
        {
            // Arrange.
            FileSystem mockFileSystem = new FileSystem();

            string testDirectory = mockFileSystem.GetTempDirectory(createDirectory: true);
            string outputDirectory = mockFileSystem.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                new TestServiceProviderOptions
                {
                    CancellationToken = cancellationTokenSource.Token,
                    FileSystem = mockFileSystem,
                    Now = now,
                });
            IDirectoriesMonitor directoriesMonitor = serviceProvider.GetRequiredService<IDirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, new OutputFilesInfo(outputDirectory));

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string oldFilePath = mockFileSystem.GetTempFile(testDirectory);
            string newFilePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(oldFilePath).DisposeAsync();
            mockFileSystem.File.Move(oldFilePath, newFilePath);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter, expectedRenamedFiles: [(oldFilePath, newFilePath)]);

            cancellationTokenSource.Cancel();
            await task;

            string renaming = $"{oldFilePath} -> {newFilePath}";
            string expectedContent =
                $"{oldFilePath}{Environment.NewLine}" +
                $"{renaming}{Environment.NewLine}";
            await mockFileSystem.CheckAllFileChangesFileAsync(outputDirectory, expectedContent);

            expectedContent =
                $"[{now}] Created: {oldFilePath}{Environment.NewLine}" +
                $"[{now}] Renamed: {oldFilePath} to {newFilePath}{Environment.NewLine}";
            await mockFileSystem.CheckEventsFileAsync(outputDirectory, expectedContent);

            string[] expectedContentLines = [renaming];
            await mockFileSystem.CheckChangesFile(outputDirectory, "RenamedFiles", expectedContentLines);
        }

        [TestMethod]
        public async Task MonitorAsync_MonitoringCurrentDirectory_OutputFileChangesAreNotAnalysed()
        {
            // Arrange.
            FileSystem mockFileSystem = new FileSystem();

            string testDirectory = mockFileSystem.GetTempDirectory(createDirectory: true);
            string outputDirectory = mockFileSystem.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            IServiceProvider serviceProvider = new MonitorCommandTestServiceProvider(
                new TestServiceProviderOptions
                {
                    CancellationToken = cancellationTokenSource.Token,
                    FileSystem = mockFileSystem,
                    Now = now,
                });
            IDirectoriesMonitor directoriesMonitor = serviceProvider.GetRequiredService<IDirectoriesMonitor>();

            // Act.
            Task task = directoriesMonitor.MonitorAsync(testDirectory, new OutputFilesInfo(outputDirectory));

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = mockFileSystem.GetTempFile(testDirectory);
            await mockFileSystem.File.Create(filePath).DisposeAsync();
            await mockFileSystem.File.WriteAllTextAsync(filePath, string.Empty);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter, expectedChangedFiles: [filePath], expectedCreatedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            stringWriter.ToString().Should().NotContain("AllFileChanges.txt");
            stringWriter.ToString().Should().NotContain("Events.txt");

            string expectedContent =
                $"[{now}] Created: {filePath}{Environment.NewLine}" +
                $"[{now}] Changed: {filePath}{Environment.NewLine}";
            await mockFileSystem.CheckEventsFileAsync(outputDirectory, expectedContent);
        }
    }
}