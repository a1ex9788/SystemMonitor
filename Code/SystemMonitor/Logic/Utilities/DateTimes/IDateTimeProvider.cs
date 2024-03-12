using System;

namespace SystemMonitor.Logic.Utilities.DateTimes
{
    internal interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }
}