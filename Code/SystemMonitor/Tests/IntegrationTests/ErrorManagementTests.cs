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

            MonitorCommandServiceProvider.ExtraRegistrationsAction = sc => sc.AddSingleton(monitorCommand);

            // Act.
            Action action = () => Program.Main(args);

            // Assert.
            action.Should().NotThrow();

            string expectedErrorsFile = "Errors.txt";
            string method = "SystemMonitor.Tests.IntegrationTests.ErrorManagementTests.<>c" +
                ".<MonitorCommand_UnexpectedError_SavesError>b__0_0(CallInfo x)";
            string expectedContent = $"System.Exception: Test exception.{Environment.NewLine}   at {method}";
            await OutputFilesChecker.CheckFile(expectedErrorsFile, expectedContent, exactContent: false);
        }
    }
}