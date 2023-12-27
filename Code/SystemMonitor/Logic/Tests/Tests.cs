using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SystemMonitor.Logic.Tests
{
    [TestClass]
    public class MonitorCommandTests
    {
        [TestMethod]
        public async Task MonitorCommand_SomeFilesCreated_PrintsExpectedResults()
        {
            // Arrange.
            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            MonitorCommand monitorCommand = new MonitorCommand();

            // Act.
            await monitorCommand.ExecuteAsync();

            // Assert.
            string output = stringWriter.ToString();

            output.Should().Be("Hola");
        }
    }
}