using FluentAssertions;
using System;
using System.IO;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.TestUtilities
{
    public static class OutputFilesChecker
    {
        public static async Task CheckEventsFileAsync(DateTime now, string expectedContent)
        {
            try
            {
                string filePath = Path.Combine(now.ToDirectoryName(), "Events.txt");

                File.Exists(filePath).Should().BeTrue();

                (await File.ReadAllTextAsync(filePath)).Should().Be(expectedContent);
            }
            catch (IOException e) when (
                e.Message.StartsWith(
                    "The process cannot access the file", StringComparison.Ordinal))
            {
                await CheckEventsFileAsync(now, expectedContent);
            }
        }
    }
}