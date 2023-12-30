using System;
using System.IO;
using System.IO.Abstractions;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic.Utilities
{
    public class OutputWriter
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IFile file;

        private readonly string OutputDirectory;
        private readonly string EventsFile;

        public OutputWriter(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.file = new FileSystem().File;

            string formattedData = dateTimeProvider.GetCurrentDateTime().ToDirectoryName();
            this.OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), formattedData);

            Directory.CreateDirectory(this.OutputDirectory);

            this.EventsFile = Path.Combine(this.OutputDirectory, "Events.txt");
        }

        public void WriteChangedFile(string filePath)
        {
            string message = this.FormatMessage($"Changed: {filePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);
        }

        public void WriteCreatedFile(string filePath)
        {
            string message = this.FormatMessage($"Created: {filePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);
        }

        public void WriteDeletedFile(string filePath)
        {
            string message = this.FormatMessage($"Deleted: {filePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);
        }

        public void WriteRenamedFile(string oldFilePath, string newFilePath)
        {
            string message = this.FormatMessage($"Renamed: {oldFilePath} to {newFilePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);
        }

        public void WriteError(string error)
        {
            string message = this.FormatMessage($"Error: {error}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);
        }

        private string FormatMessage(string message)
        {
            return $"[{this.dateTimeProvider.GetCurrentDateTime()}] {message}";
        }

        private void AppendToEventsFile(string message)
        {
            message += Environment.NewLine;

            this.file.AppendAllText(this.EventsFile, message);
        }
    }
}