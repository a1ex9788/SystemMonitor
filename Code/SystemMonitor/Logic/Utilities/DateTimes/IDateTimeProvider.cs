using System;

namespace SystemMonitor.Logic.Utilities.DateTimes
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }
}