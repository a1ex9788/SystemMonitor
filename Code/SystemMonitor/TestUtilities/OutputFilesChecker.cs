using FluentAssertions;
using System;
using System.IO;
using System.Threading.Tasks;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.TestUtilities
{
    public static class OutputFilesChecker
    {
        public static async Task CheckEventsFileAsync(DateTime now, string content)
        {
            string filePath = Path.Combine(now.ToDirectoryName(), "Events.txt");

            File.Exists(filePath).Should().BeTrue();

            (await File.ReadAllTextAsync(filePath)).Should().Be(content);
        }
    }
}