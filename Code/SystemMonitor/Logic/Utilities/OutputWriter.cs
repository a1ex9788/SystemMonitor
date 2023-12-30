using System;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic.Utilities
{
    public class OutputWriter(IDateTimeProvider dateTimeProvider)
    {
        public void WriteChangedFile(string filePath)
        {
            string message = this.FormatMessage($"Changed: {filePath}");

            Console.WriteLine(message);
        }

        public void WriteCreatedFile(string filePath)
        {
            string message = this.FormatMessage($"Created: {filePath}");

            Console.WriteLine(message);
        }

        public void WriteDeletedFile(string filePath)
        {
            string message = this.FormatMessage($"Deleted: {filePath}");

            Console.WriteLine(message);
        }

        public void WriteRenamedFile(string oldFilePath, string newFilePath)
        {
            string message = this.FormatMessage($"Renamed: {oldFilePath} to {newFilePath}");

            Console.WriteLine(message);
        }

        public void WriteError(string error)
        {
            string message = this.FormatMessage($"Error: {error}");

            Console.WriteLine(message);
        }

        private string FormatMessage(string message)
        {
            return $"[{dateTimeProvider.GetCurrentDateTime()}] {message}";
        }
    }
}