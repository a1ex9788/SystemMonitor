using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SystemMonitor.Logic.Utilities
{
    internal static class DrivesObtainer
    {
        internal static IEnumerable<DriveInfo> GetDrives()
        {
            return DriveInfo.GetDrives().Where(di => di.DriveType == DriveType.Fixed);
        }
    }
}