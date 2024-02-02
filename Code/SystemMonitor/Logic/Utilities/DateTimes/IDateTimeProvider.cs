using System;

namespace SystemMonitor.Logic.Utilities.DateTimes
{
    internal interface IDateTimeProvider
    {
        internal DateTime GetCurrentDateTime();
    }
}