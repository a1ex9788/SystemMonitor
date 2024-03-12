using System.Collections.Generic;

namespace SystemMonitor.Logic.Utilities.Drives
{
    internal interface IDrivesObtainer
    {
        IReadOnlyCollection<Drive> GetDrives();
    }
}