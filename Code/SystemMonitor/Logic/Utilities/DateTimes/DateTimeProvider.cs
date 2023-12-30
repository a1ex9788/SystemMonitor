using System;

namespace SystemMonitor.Logic.Utilities.DateTimes
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}