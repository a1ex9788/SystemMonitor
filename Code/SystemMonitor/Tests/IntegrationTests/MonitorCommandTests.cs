using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemMonitor.Logic;
using SystemMonitor.TestsUtilities;

namespace SystemMonitor.Tests.IntegrationTests
{
    [TestClass]
    public class MonitorCommandTests
    {
        [TestMethod]
        public async Task MonitorCommand_SomeFileChanges_PrintsFileChanges()
        {
            // Arrange.
            string[] args = [];
            string testDirectory = TempPathsObtainer.GetTempDirectory();

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            MonitorCommandServiceProvider.ExtraRegistrationsAction =
                sc =>
                {
                    sc.AddSingleton(typeof(CancellationToken), cancellationTokenSource.Token);
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
            string testPath = TempPathsObtainer.GetTempDirectory();
            string[] args = ["-d", testPath];

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
            passedDirectory.Should().Be(testPath);
        }

        [TestMethod]
        public void MonitorCommand_DirectoryOptionSpecifiedButMissingValue_PrintsError()
        {
            // Arrange.
            string testPath = TempPathsObtainer.GetTempDirectory();
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
    }
}