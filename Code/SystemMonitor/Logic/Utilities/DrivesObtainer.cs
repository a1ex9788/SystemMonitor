using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SystemMonitor.Logic.Utilities
{
    internal static class DrivesObtainer
    {
        internal static IEnumerable<string> GetDrives()
        {
            return DriveInfo.GetDrives()
                .Where(di => di.DriveType == DriveType.Fixed)
                .Select(di => di.Name);
        }
    }
}