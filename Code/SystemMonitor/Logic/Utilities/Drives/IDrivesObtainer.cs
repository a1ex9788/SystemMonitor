using System.Collections.Generic;
using System.IO;

namespace SystemMonitor.Logic.Utilities.Drives
{
    internal interface IDrivesObtainer
    {
        IReadOnlyCollection<DriveInfo> GetDrives();
    }
}