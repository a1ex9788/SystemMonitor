using System;
using System.IO;

namespace SystemMonitor.Logic.FileSystemWatchers
{
    internal interface IFileSystemWatcher : IDisposable
    {
        event FileSystemEventHandler? Changed;

        event FileSystemEventHandler? Created;

        event FileSystemEventHandler? Deleted;

        event RenamedEventHandler? Renamed;

        event ErrorEventHandler? Error;
    }
}