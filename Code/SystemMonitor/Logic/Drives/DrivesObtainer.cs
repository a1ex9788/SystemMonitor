using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SystemMonitor.Logic.Drives
{
    internal class DrivesObtainer : IDrivesObtainer
    {
        public IReadOnlyCollection<Drive> GetDrives()
        {
            return DriveInfo.GetDrives()
                .Where(di => di.DriveType == DriveType.Fixed)
                .Select(di => new Drive(di.VolumeLabel, di.RootDirectory.FullName))
                .ToArray();
        }
    }
}