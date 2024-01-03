using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public static async Task CheckChangesFile(
            DateTime now, string changesFileName, IEnumerable<string> expectedContentLines)
        {
            string filePath = Path.Combine(
                now.ToDirectoryName(), "FileChanges", $"{changesFileName}.txt");

            File.Exists(filePath).Should().BeTrue();

            StringBuilder stringBuilder = new StringBuilder();

            foreach (string line in expectedContentLines)
            {
                stringBuilder.AppendLine(line);
            }

            (await File.ReadAllTextAsync(filePath)).Should().Be(stringBuilder.ToString());
        }
    }
}