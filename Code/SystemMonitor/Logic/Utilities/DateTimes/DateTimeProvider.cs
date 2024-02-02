using System;

namespace SystemMonitor.Logic.Utilities.DateTimes
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}