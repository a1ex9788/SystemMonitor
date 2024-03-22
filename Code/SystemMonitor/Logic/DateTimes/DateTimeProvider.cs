using System;

namespace SystemMonitor.Logic.DateTimes
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}