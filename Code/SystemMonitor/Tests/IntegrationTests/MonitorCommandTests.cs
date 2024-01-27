using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic;
using SystemMonitor.Logic.Utilities.DateTimes;
using SystemMonitor.TestUtilities;

namespace SystemMonitor.Tests.IntegrationTests
{
    [TestClass]
    public class MonitorCommandTests
    {
        [TestMethod]
        public async Task MonitorCommand_SomeFileChanges_PrintsAndSavesFileChanges()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string[] args = ["-d", testDirectory];

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            DateTime now = RandomDateTimeGenerator.Get();
            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc =>
                {
                    sc.AddSingleton(typeof(CancellationToken), cancellationTokenSource.Token);
                    sc.AddScoped<IDateTimeProvider>(_ => new DateTimeProviderFake(now));
                };

            // Act.
            Task task = Task.Run(() => Program.Main(args));

            await EventsWaiter.WaitForEventsRegistrationAsync(stringWriter);

            string filePath = TempPathsObtainer.GetTempFile(testDirectory);

            await File.Create(filePath).DisposeAsync();
            await File.WriteAllTextAsync(filePath, string.Empty);

            // Assert.
            await EventsWaiter.WaitForEventsProsecutionAsync(
                stringWriter,
                expectedCreatedFiles: [filePath],
                expectedChangedFiles: [filePath]);

            cancellationTokenSource.Cancel();
            await task;

            string outputDirectory = Path.Combine(now.ToDirectoryName());
            string expectedContent =
                $"[{now}] Created: {filePath}{Environment.NewLine}" +
                $"[{now}] Changed: {filePath}{Environment.NewLine}";
            await OutputFilesChecker.CheckEventsFileAsync(outputDirectory, expectedContent);
        }

        [TestMethod]
        public void MonitorCommand_DirectoryNotSpecified_NullStringPassed()
        {
            // Arrange.
            string[] args = [];

            string? passedDirectory = null;

            IMonitorCommand monitorCommand = Substitute.For<IMonitorCommand>();
            monitorCommand
                .ExecuteAsync(Arg.Any<string?>())
                .Returns(x =>
                {
                    passedDirectory = x.Args()[0] as string;

                    return Task.CompletedTask;
                });

            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc => sc.AddSingleton(monitorCommand);

            // Act.
            Program.Main(args);

            // Assert.
            passedDirectory.Should().Be(null);
        }

        [TestMethod]
        public void MonitorCommand_DirectorySpecified_DirectoryPassed()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string[] args = ["-d", testDirectory];

            string? passedDirectory = null;

            IMonitorCommand monitorCommand = Substitute.For<IMonitorCommand>();
            monitorCommand
                .ExecuteAsync(Arg.Any<string?>())
                .Returns(x =>
                {
                    passedDirectory = x.Args()[0] as string;

                    return Task.CompletedTask;
                });

            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc => sc.AddSingleton(monitorCommand);

            // Act.
            Program.Main(args);

            // Assert.
            passedDirectory.Should().Be(testDirectory);
        }

        [TestMethod]
        public void MonitorCommand_DirectoryOptionSpecifiedButMissingValue_PrintsError()
        {
            // Arrange.
            string testDirectory = TempPathsObtainer.GetTempDirectory();
            string[] args = ["-d"];

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            bool commandExecuted = false;

            IMonitorCommand monitorCommand = Substitute.For<IMonitorCommand>();
            monitorCommand
                .ExecuteAsync(Arg.Any<string?>())
                .Returns(x =>
                {
                    commandExecuted = true;

                    return Task.CompletedTask;
                });

            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc => sc.AddSingleton(monitorCommand);

            // Act.
            Action action = () => Program.Main(args);

            // Assert.
            action.Should().NotThrow();

            commandExecuted.Should().BeFalse();

            string expectedOutput =
               $"Specify --help for a list of available options and commands.{Environment.NewLine}";
            stringWriter.ToString().Should().Be(expectedOutput);
        }

        [TestMethod]
        public async Task MonitorCommand_UnexpectedError_SavesError()
        {
            // Arrange.
            string[] args = [];

            IMonitorCommand monitorCommand = Substitute.For<IMonitorCommand>();
            monitorCommand
                .ExecuteAsync(Arg.Any<string?>())
                .Returns(x =>
                {
                    throw new Exception("Test exception.");
                });

            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc => sc.AddSingleton(monitorCommand);

            // Act.
            Action action = () => Program.Main(args);

            // Assert.
            action.Should().NotThrow();

            string expectedErrorsFile = "Errors.txt";
            string method = "SystemMonitor.Tests.IntegrationTests.MonitorCommandTests.<>c" +
                ".<MonitorCommand_UnexpectedError_SavesError>b__4_0(CallInfo x)";
            string expectedContent =
               $"System.Exception: Test exception.{Environment.NewLine}" +
               $"   at {method}";
            await OutputFilesChecker.CheckFile(
                expectedErrorsFile, expectedContent, exactContent: false);
        }
    }
}