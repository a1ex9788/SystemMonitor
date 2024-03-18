using CliWrap;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Tests.IntegrationTests
{
    [TestClass]
    public class ToolInvocationTests
    {
        [TestMethod]
        public async Task Tool_AbbreviatedName_ToolExecutedCorrectly()
        {
            // Arrange.
            StringBuilder stringBuilder = new StringBuilder();

            Command command = Cli.Wrap("sm")
                .WithArguments("--help")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stringBuilder));

            // Act.
            await command.ExecuteAsync();

            // Assert.
            stringBuilder.ToString().Should().Contain("Usage: systemMonitor [options]");
        }
    }
}