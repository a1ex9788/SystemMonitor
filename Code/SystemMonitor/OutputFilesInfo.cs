using System;
using System.IO;

namespace SystemMonitor
{
    internal class OutputFilesInfo
    {
        public const string allFileChangesFileName = "AllFileChanges.txt";
        public const string eventsFileName = "Events.txt";

        public OutputFilesInfo(string outputDirectoryFullPath, string? toolOutputDirectoryFullPath = null)
        {
            CheckFullyQualifiedDirectory(outputDirectoryFullPath);

            this.OutputDirectory = outputDirectoryFullPath;

            if (toolOutputDirectoryFullPath is null)
            {
                this.ToolOutputDirectory = this.OutputDirectory;
            }
            else
            {
                CheckFullyQualifiedDirectory(toolOutputDirectoryFullPath);

                this.ToolOutputDirectory = toolOutputDirectoryFullPath;
            }

            static void CheckFullyQualifiedDirectory(string directory)
            {
                if (!Path.IsPathFullyQualified(directory))
                {
                    throw new ArgumentException($"Directory '{directory}' must be fully qualified.");
                }
            }

            this.FileChangesDirectory = Path.Combine(this.OutputDirectory, "FileChanges");
            this.ChangedFilesFile = Path.Combine(this.FileChangesDirectory, "ChangedFiles.txt");
            this.CreatedFilesFile = Path.Combine(this.FileChangesDirectory, "CreatedFiles.txt");
            this.DeletedFilesFile = Path.Combine(this.FileChangesDirectory, "DeletedFiles.txt");
            this.RenamedFilesFile = Path.Combine(this.FileChangesDirectory, "RenamedFiles.txt");

            this.AllFileChangesFile = Path.Combine(this.OutputDirectory, allFileChangesFileName);
            this.EventsFile = Path.Combine(this.OutputDirectory, eventsFileName);

            if (this.OutputDirectory != this.ToolOutputDirectory)
            {
                this.GeneralAllFileChangesFile = Path.Combine(this.ToolOutputDirectory, allFileChangesFileName);
                this.GeneralEventsFile = Path.Combine(this.ToolOutputDirectory, eventsFileName);
            }
        }

        public string OutputDirectory { get; }

        public string ToolOutputDirectory { get; }

        // Output files:
        // - FileChanges
        //     - ChangedFiles.txt
        //     - CreatedFiles.txt
        //     - DeletedFiles.txt
        //     - RenamedFiles.txt
        // - AllFileChanges.txt
        // - Events.txt

        public string FileChangesDirectory { get; }

        public string ChangedFilesFile { get; }

        public string CreatedFilesFile { get; }

        public string DeletedFilesFile { get; }

        public string RenamedFilesFile { get; }

        public string AllFileChangesFile { get; }

        public string EventsFile { get; }

        // Extra optional output files.

        public string? GeneralAllFileChangesFile { get; }

        public string? GeneralEventsFile { get; }
    }
}