﻿using System;
using System.IO;
using System.IO.Abstractions;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic.Utilities
{
    public class OutputWriter
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IFile file;

        private readonly string outputDirectory;
        private readonly string fileChangesDirectory;
        private readonly string eventsFile;
        private readonly string allFileChangesFile;
        private readonly string changedFilesFile;
        private readonly string createdFilesFile;
        private readonly string deletedFilesFile;
        private readonly string renamedFilesFile;

        public OutputWriter(string outputDirectory, IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.file = new FileSystem().File;

            this.outputDirectory = outputDirectory;
            Directory.CreateDirectory(this.outputDirectory);

            this.fileChangesDirectory = Path.Combine(this.outputDirectory, "FileChanges");
            Directory.CreateDirectory(this.fileChangesDirectory);

            this.eventsFile = Path.Combine(this.outputDirectory, "Events.txt");
            this.allFileChangesFile = Path.Combine(this.fileChangesDirectory, "AllFileChanges.txt");
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

            this.AppendToEventsFile(message);

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

            this.AppendToEventsFile(message);

            this.AddFilePathToChangesFile(this.createdFilesFile, filePath);
        }

        public void WriteDeletedFile(string filePath)
        {
            string message = this.FormatMessage($"Deleted: {filePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);

            this.AddFilePathToChangesFile(this.deletedFilesFile, filePath);
        }

        public void WriteRenamedFile(string oldFilePath, string newFilePath)
        {
            string message = this.FormatMessage($"Renamed: {oldFilePath} to {newFilePath}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);

            this.AddFilePathToChangesFile(this.renamedFilesFile, $"{oldFilePath} -> {newFilePath}");
        }

        public void WriteError(string error)
        {
            string message = this.FormatMessage($"Error: {error}");

            Console.WriteLine(message);

            this.AppendToEventsFile(message);
        }

        private bool IsOutputFile(string filePath)
        {
            return filePath.StartsWith(this.outputDirectory);
        }

        private string FormatMessage(string message)
        {
            return $"[{this.dateTimeProvider.GetCurrentDateTime()}] {message}";
        }

        private void AppendToEventsFile(string message)
        {
            message += Environment.NewLine;

            this.file.AppendAllText(this.eventsFile, message);
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