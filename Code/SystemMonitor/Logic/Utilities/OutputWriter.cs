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
        private readonly string FileChangesDirectory;
        private readonly string EventsFile;
        private readonly string AllFileChangesFile;
        private readonly string ChangedFilesFile;
        private readonly string CreatedFilesFile;
        private readonly string DeletedFilesFile;
        private readonly string RenamedFilesFile;

        public OutputWriter(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.file = new FileSystem().File;

            string formattedData = dateTimeProvider.GetCurrentDateTime().ToDirectoryName();
            this.OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), formattedData);
            Directory.CreateDirectory(this.OutputDirectory);

            this.FileChangesDirectory = Path.Combine(this.OutputDirectory, "FileChanges");
            Directory.CreateDirectory(this.FileChangesDirectory);

            this.EventsFile = Path.Combine(this.OutputDirectory, "Events.txt");
            this.AllFileChangesFile = Path.Combine(this.FileChangesDirectory, "AllFileChanges.txt");
            this.ChangedFilesFile = Path.Combine(this.FileChangesDirectory, "ChangedFiles.txt");
            this.CreatedFilesFile = Path.Combine(this.FileChangesDirectory, "CreatedFiles.txt");
            this.DeletedFilesFile = Path.Combine(this.FileChangesDirectory, "DeletedFiles.txt");
            this.RenamedFilesFile = Path.Combine(this.FileChangesDirectory, "RenamedFiles.txt");
        }

        public void WriteChangedFile(string filePath)
        {
            if (this.IsOutputFile(filePath))
            {
                return;
            }

            string message = this.FormatMessage($"Changed: {filePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);

            this.AddFilePathToChangesFile(this.ChangedFilesFile, filePath);
        }

        public void WriteCreatedFile(string filePath)
        {
            if (this.IsOutputFile(filePath))
            {
                return;
            }

            string message = this.FormatMessage($"Created: {filePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);

            this.AddFilePathToChangesFile(this.CreatedFilesFile, filePath);
        }

        public void WriteDeletedFile(string filePath)
        {
            string message = this.FormatMessage($"Deleted: {filePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);

            this.AddFilePathToChangesFile(this.DeletedFilesFile, filePath);
        }

        public void WriteRenamedFile(string oldFilePath, string newFilePath)
        {
            string message = this.FormatMessage($"Renamed: {oldFilePath} to {newFilePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);

            this.AddFilePathToChangesFile(this.RenamedFilesFile, $"{oldFilePath} -> {newFilePath}");
        }

        public void WriteError(string error)
        {
            string message = this.FormatMessage($"Error: {error}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);
        }

        private bool IsOutputFile(string filePath)
        {
            return filePath.StartsWith(this.OutputDirectory);
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

        private void AddFilePathToChangesFile(string changesFile, string filePath)
        {
            string[] newContent;

            if (this.file.Exists(changesFile))
            {
                string[] content = this.file.ReadAllLines(changesFile);

                OrderedStringList orderedStringList = new OrderedStringList(content);
                orderedStringList.AddIfNotExist(filePath);

                newContent = orderedStringList.Items;
            }
            else
            {
                newContent = [filePath];
            }

            this.file.WriteAllLines(changesFile, newContent);
        }
    }
}