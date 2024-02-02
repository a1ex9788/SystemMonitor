using System;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Tests.Utilities
{
    internal class DateTimeProviderFake(DateTime now) : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return now;
        }
    }
}