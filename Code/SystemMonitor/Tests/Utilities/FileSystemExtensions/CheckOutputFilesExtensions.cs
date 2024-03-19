using FluentAssertions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Abstractions
{
    internal static class CheckOutputFilesExtensions
    {
        public static async Task CheckAllFileChangesFileAsync(
            this IFileSystem fileSystem, string outputDirectory, string expectedContent, bool exactContent = true)
        {
            string filePath = Path.Combine(outputDirectory, "AllFileChanges.txt");

            await fileSystem.CheckFile(filePath, expectedContent, exactContent);
        }

        public static async Task CheckEventsFileAsync(
            this IFileSystem fileSystem, string outputDirectory, string expectedContent, bool exactContent = true)
        {
            try
            {
                string filePath = Path.Combine(outputDirectory, "Events.txt");

                await fileSystem.CheckFile(filePath, expectedContent, exactContent);
            }
            catch (IOException e)
                when (e.Message.StartsWith("The process cannot access the file", StringComparison.Ordinal))
            {
                await fileSystem.CheckEventsFileAsync(outputDirectory, expectedContent);
            }
        }

        public static async Task CheckChangesFile(
            this IFileSystem fileSystem,
            string outputDirectory,
            string changesFileName,
            IReadOnlyCollection<string> expectedContentLines)
        {
            string filePath = Path.Combine(outputDirectory, "FileChanges", $"{changesFileName}.txt");

            StringBuilder stringBuilder = new StringBuilder();

            foreach (string line in expectedContentLines)
            {
                stringBuilder.AppendLine(line);
            }

            await fileSystem.CheckFile(filePath, stringBuilder.ToString());
        }

        public static async Task CheckFile(
            this IFileSystem fileSystem, string filePath, string expectedContent, bool exactContent = true)
        {
            fileSystem.File.Exists(filePath).Should().BeTrue();

            string content = await fileSystem.File.ReadAllTextAsync(filePath);

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