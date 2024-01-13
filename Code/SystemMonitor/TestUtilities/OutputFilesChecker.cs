using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.TestUtilities
{
    public static class OutputFilesChecker
    {
        public static async Task CheckAllFileChangesFileAsync(
            string outputDirectory, string expectedContent, bool exactContent = true)
        {
            string filePath = Path.Combine(outputDirectory, "AllFileChanges.txt");

            File.Exists(filePath).Should().BeTrue();

            string content = await File.ReadAllTextAsync(filePath);

            if (exactContent)
            {
                content.Should().Be(expectedContent);
            }
            else
            {
                content.Should().Contain(expectedContent);
            }
        }

        public static async Task CheckEventsFileAsync(
            string outputDirectory, string expectedContent, bool exactContent = true)
        {
            try
            {
                string filePath = Path.Combine(outputDirectory, "Events.txt");

                File.Exists(filePath).Should().BeTrue();

                string content = await File.ReadAllTextAsync(filePath);

                if (exactContent)
                {
                    content.Should().Be(expectedContent);
                }
                else
                {
                    content.Should().Contain(expectedContent);
                }
            }
            catch (IOException e) when (
                e.Message.StartsWith(
                    "The process cannot access the file", StringComparison.Ordinal))
            {
                await CheckEventsFileAsync(outputDirectory, expectedContent);
            }
        }

        public static async Task CheckChangesFile(
            string outputDirectory,
            string changesFileName,
            IEnumerable<string> expectedContentLines)
        {
            string filePath = Path.Combine(
                outputDirectory, "FileChanges", $"{changesFileName}.txt");

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