using System;

namespace SystemMonitor.Logic.DateTimes
{
    internal interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }
}