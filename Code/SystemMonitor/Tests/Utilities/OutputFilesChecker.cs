using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Tests.Utilities
{
    internal static class OutputFilesChecker
    {
        internal static async Task CheckAllFileChangesFileAsync(
            string outputDirectory, string expectedContent, bool exactContent = true)
        {
            string filePath = Path.Combine(outputDirectory, "AllFileChanges.txt");

            await CheckFile(filePath, expectedContent, exactContent);
        }

        internal static async Task CheckEventsFileAsync(
            string outputDirectory, string expectedContent, bool exactContent = true)
        {
            try
            {
                string filePath = Path.Combine(outputDirectory, "Events.txt");

                await CheckFile(filePath, expectedContent, exactContent);
            }
            catch (IOException e) when (
                e.Message.StartsWith(
                    "The process cannot access the file", StringComparison.Ordinal))
            {
                await CheckEventsFileAsync(outputDirectory, expectedContent);
            }
        }

        internal static async Task CheckChangesFile(
            string outputDirectory,
            string changesFileName,
            IEnumerable<string> expectedContentLines)
        {
            string filePath = Path.Combine(
                outputDirectory, "FileChanges", $"{changesFileName}.txt");

            StringBuilder stringBuilder = new StringBuilder();

            foreach (string line in expectedContentLines)
            {
                stringBuilder.AppendLine(line);
            }

            await CheckFile(filePath, stringBuilder.ToString());
        }

        internal static async Task CheckFile(
            string filePath, string expectedContent, bool exactContent = true)
        {
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
    }
}