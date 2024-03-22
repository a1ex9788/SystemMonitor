using System;
using System.IO.Abstractions;
using SystemMonitor.Logic.Collections;
using SystemMonitor.Logic.DateTimes;

namespace SystemMonitor.Logic.Output
{
    internal class OutputWriter : IOutputWriter
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IFile file;
        private readonly OutputFilesInfo outputFilesInfo;

        public OutputWriter(
            IDateTimeProvider dateTimeProvider, IDirectory directory, IFile file, OutputFilesInfo outputFilesInfo)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.file = file;
            this.outputFilesInfo = outputFilesInfo;

            directory.CreateDirectory(this.outputFilesInfo.OutputDirectory);
            directory.CreateDirectory(this.outputFilesInfo.FileChangesDirectory);
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
            this.AddFilePathToChangesFileIfNeeded(this.outputFilesInfo.GeneralAllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.outputFilesInfo.AllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.outputFilesInfo.ChangedFilesFile, filePath);
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
            this.AddFilePathToChangesFileIfNeeded(this.outputFilesInfo.GeneralAllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.outputFilesInfo.AllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.outputFilesInfo.CreatedFilesFile, filePath);
        }

        public void WriteDeletedFile(string filePath)
        {
            string message = this.FormatMessage($"Deleted: {filePath}");

            Console.WriteLine(message);

            this.AppendToGeneralEventsFileIfNeeded(message);
            this.AppendToEventsFile(message);
            this.AddFilePathToChangesFileIfNeeded(this.outputFilesInfo.GeneralAllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.outputFilesInfo.AllFileChangesFile, filePath);
            this.AddFilePathToChangesFile(this.outputFilesInfo.DeletedFilesFile, filePath);
        }

        public void WriteRenamedFile(string oldFilePath, string newFilePath)
        {
            string message = this.FormatMessage($"Renamed: {oldFilePath} to {newFilePath}");

            Console.WriteLine(message);

            this.AppendToGeneralEventsFileIfNeeded(message);
            this.AppendToEventsFile(message);

            string renaming = $"{oldFilePath} -> {newFilePath}";
            this.AddFilePathToChangesFileIfNeeded(this.outputFilesInfo.GeneralAllFileChangesFile, renaming);
            this.AddFilePathToChangesFile(this.outputFilesInfo.AllFileChangesFile, renaming);
            this.AddFilePathToChangesFile(this.outputFilesInfo.RenamedFilesFile, renaming);
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
            return filePath.StartsWith(this.outputFilesInfo.ToolOutputDirectory);
        }

        private string FormatMessage(string message)
        {
            return $"[{this.dateTimeProvider.GetCurrentDateTime()}] {message}";
        }

        private void AppendToGeneralEventsFileIfNeeded(string message)
        {
            if (this.outputFilesInfo.GeneralEventsFile is null)
            {
                return;
            }

            message += Environment.NewLine;

            this.file.AppendAllText(this.outputFilesInfo.GeneralEventsFile, message);
        }

        private void AppendToEventsFile(string message)
        {
            message += Environment.NewLine;

            this.file.AppendAllText(this.outputFilesInfo.EventsFile, message);
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