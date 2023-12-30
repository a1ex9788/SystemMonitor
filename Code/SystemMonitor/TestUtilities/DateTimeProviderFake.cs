using System;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.TestUtilities
{
    public class DateTimeProviderFake(DateTime now) : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return now;
        }
    }
}