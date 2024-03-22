namespace SystemMonitor.Logic.Output
{
    internal interface IOutputWriter
    {
        void WriteChangedFile(string filePath);

        void WriteCreatedFile(string filePath);

        void WriteDeletedFile(string filePath);

        void WriteRenamedFile(string oldFilePath, string newFilePath);

        void WriteError(string error);
    }
}