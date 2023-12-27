using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace SystemMonitor.Tests
{
    [TestClass]
    public class MonitorCommandTests
    {
        [TestMethod]
        public void MonitorCommand_SomeFilesCreated_PrintsExpectedResults()
        {
            // Arrange.
            string[] args = [];

            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act.
            Program.Main(args);

            // Assert.
            string output = stringWriter.ToString();

            output.Should().Be("Hola");
        }
    }
}