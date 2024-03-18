namespace SystemMonitor.Exceptions
{
    internal class NotExistingDirectoryException(string directoryFullPath)
        : SystemMonitorException($"Directory '{directoryFullPath}' does not exist.")
    {
    }
}