using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SystemMonitor.Logic.Utilities.Drives
{
    internal class DrivesObtainer : IDrivesObtainer
    {
        public IReadOnlyCollection<DriveInfo> GetDrives()
        {
            return DriveInfo.GetDrives().Where(di => di.DriveType == DriveType.Fixed).ToArray();
        }
    }
}