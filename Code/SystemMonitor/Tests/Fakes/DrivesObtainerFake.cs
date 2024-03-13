using System.Collections.Generic;
using SystemMonitor.Logic.Utilities.Drives;

namespace SystemMonitor.Tests.Fakes
{
    internal class DrivesObtainerFake(IReadOnlyCollection<Drive> drives) : IDrivesObtainer
    {
        public IReadOnlyCollection<Drive> GetDrives()
        {
            return drives;
        }
    }
}