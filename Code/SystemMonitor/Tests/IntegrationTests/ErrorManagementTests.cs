using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading.Tasks;
using SystemMonitor.Logic;
using SystemMonitor.Tests.Utilities;

namespace SystemMonitor.Tests.IntegrationTests
{
    [TestClass]
    public class ErrorManagementTests
    {
        [TestMethod]
        public void Tool_NoError_ReturnsSuccessExitCode()
        {
            // Arrange.
            bool executed = false;

            IMonitorCommand monitorCommand = Substitute.For<IMonitorCommand>();
            monitorCommand
                .ExecuteAsync(Arg.Any<string?>())
                .Returns(_ =>
                {
                    executed = true;

                    return Task.CompletedTask;
                });

            MonitorCommandServiceProvider.ExtraRegistrationsAction = sc => sc.AddSingleton(monitorCommand);

            // Act.
            static int Function() => Program.Main(args: []);

            // Assert.
            Function().Should().Be(0);

            executed.Should().BeTrue();
        }

        [TestMethod]
        public async Task Tool_UnexpectedError_SavesErrorAndReturnsErrorExitCode()
        {
            // Arrange.
            IMonitorCommand monitorCommand = Substitute.For<IMonitorCommand>();
            monitorCommand
                .ExecuteAsync(Arg.Any<string?>())
                .Returns(x =>
                {
                    throw new Exception("Test exception.");
                });

            MonitorCommandServiceProvider.ExtraRegistrationsAction = sc => sc.AddSingleton(monitorCommand);

            // Act.
            static int Function() => Program.Main(args: []);

            // Assert.
            Function().Should().Be(-1);

            string expectedErrorsFile = "Errors.txt";
            string method = $"{typeof(ErrorManagementTests).FullName}.<>c" +
                $".<{nameof(Tool_UnexpectedError_SavesErrorAndReturnsErrorExitCode)}>b__1_0(CallInfo x)";
            string expectedContent = $"System.Exception: Test exception.{Environment.NewLine}   at {method}";
            await OutputFilesChecker.CheckFile(expectedErrorsFile, expectedContent, exactContent: false);
        }
    }
}