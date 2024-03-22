using System.Collections.Generic;

namespace SystemMonitor.Logic.Drives
{
    internal interface IDrivesObtainer
    {
        IReadOnlyCollection<Drive> GetDrives();
    }
}