using System;
using System.IO;
using System.IO.Abstractions;
using SystemMonitor.Logic.Utilities;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic
{
    internal class OutputWriter
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IFile file;

        private readonly string baseOutputDirectory;
        private readonly string fileChangesDirectory;
        private readonly string? generalAllFileChangesFile;
        private readonly string? generalEventsFile;
        private readonly string eventsFile;
        private readonly string allFileChangesFile;
        private readonly string changedFilesFile;
        private readonly string createdFilesFile;
        private readonly string deletedFilesFile;
        private readonly string renamedFilesFile;

        public static readonly string AllFileChangesFileName = "AllFileChanges.txt";
        public static readonly string EventsFileName = "Events.txt";

        public OutputWriter(
            string baseOutputDirectory,
            string outputDirectory,
            IDateTimeProvider dateTimeProvider,
            string? generalAllFileChangesFile = null,
            string? generalEventsFile = null)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.file = new FileSystem().File;

            this.baseOutputDirectory = baseOutputDirectory;
            Directory.CreateDirectory(outputDirectory);

            this.fileChangesDirectory = Path.Combine(outputDirectory, "FileChanges");
            Directory.CreateDirectory(this.fileChangesDirectory);

            this.generalAllFileChangesFile = generalAllFileChangesFile;
            this.generalEventsFile = generalEventsFile;
            this.eventsFile = Path.Combine(outputDirectory, EventsFileName);
            this.allFileChangesFile = Path.Combine(outputDirectory, "AllFileChanges.txt");
            this.changedFilesFile = Path.Combine(this.fileChangesDirectory, "ChangedFiles.txt");
            this.createdFilesFile = Path.Combine(this.fileChangesDirectory, "CreatedFiles.txt");
            this.deletedFilesFile = Path.Combine(this.fileChangesDirectory, "DeletedFiles.txt");
            this.renamedFilesFile = Path.Combine(this.fileChangesDirectory, "RenamedFiles.txt");
        }

        public void WriteChangedFile(string filePath)
        {
            if (this.IsOutputFile(filePath))
            {
                return;
            }

            string message = this.FormatMessage($"Changed: {filePath}");

            Console.WriteLine(message);

            this.AppendToGeneralEventsFileIfNeeded(message);
            this.AppendToEventsFile(message);
            this.AddFilePathToChangesFileIfNeeded(this.generalAllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.allFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.changedFilesFile, filePath);
        }

        public void WriteCreatedFile(string filePath)
        {
            if (this.IsOutputFile(filePath))
            {
                return;
            }

            string message = this.FormatMessage($"Created: {filePath}");

            Console.WriteLine(message);

            this.AppendToGeneralEventsFileIfNeeded(message);
            this.AppendToEventsFile(message);
            this.AddFilePathToChangesFileIfNeeded(this.generalAllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.allFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.createdFilesFile, filePath);
        }

        public void WriteDeletedFile(string filePath)
        {
            string message = this.FormatMessage($"Deleted: {filePath}");

            Console.WriteLine(message);

            this.AppendToGeneralEventsFileIfNeeded(message);
            this.AppendToEventsFile(message);
            this.AddFilePathToChangesFileIfNeeded(this.generalAllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.allFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.deletedFilesFile, filePath);
        }

        public void WriteRenamedFile(string oldFilePath, string newFilePath)
        {
            string message = this.FormatMessage($"Renamed: {oldFilePath} to {newFilePath}");

            Console.WriteLine(message);

            this.AppendToGeneralEventsFileIfNeeded(message);
            this.AppendToEventsFile(message);

            string renaming = $"{oldFilePath} -> {newFilePath}";
            this.AddFilePathToChangesFileIfNeeded(this.generalAllFileChangesFile, renaming);
            this.AddFilePathToChangesFile(this.allFileChangesFile, renaming);
            this.AddFilePathToChangesFile(this.renamedFilesFile, renaming);
        }

        public void WriteError(string error)
        {
            string message = this.FormatMessage($"Error: {error}");

            Console.WriteLine(message);

            this.AppendToGeneralEventsFileIfNeeded(message);
            this.AppendToEventsFile(message);
        }

        private bool IsOutputFile(string filePath)
        {
            return filePath.StartsWith(this.baseOutputDirectory);
        }

        private string FormatMessage(string message)
        {
            return $"[{this.dateTimeProvider.GetCurrentDateTime()}] {message}";
        }

        private void AppendToGeneralEventsFileIfNeeded(string message)
        {
            if (this.generalEventsFile is null)
            {
                return;
            }

            message += Environment.NewLine;

            this.file.AppendAllText(this.generalEventsFile, message);
        }

        private void AppendToEventsFile(string message)
        {
            message += Environment.NewLine;

            this.file.AppendAllText(this.eventsFile, message);
        }

        private void AddFilePathToChangesFileIfNeeded(string? changesFile, string filePath)
        {
            if (changesFile is null)
            {
                return;
            }

            this.AddFilePathToChangesFile(changesFile, filePath);
        }

        private void AddFilePathToChangesFile(string changesFile, string filePath)
        {
            string[] newContent;

            if (this.file.Exists(changesFile))
            {
                string[] content = this.file.ReadAllLines(changesFile);

                OrderedStringArray orderedStringArray = new OrderedStringArray(content);
                bool added = orderedStringArray.AddIfNotExist(filePath);

                if (!added)
                {
                    return;
                }

                newContent = orderedStringArray.GetItems();
            }
            else
            {
                newContent = [filePath];
            }

            this.file.WriteAllLines(changesFile, newContent);
        }
    }
}