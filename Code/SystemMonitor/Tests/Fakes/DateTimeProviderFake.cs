using System;
using SystemMonitor.Logic.DateTimes;

namespace SystemMonitor.Tests.Fakes
{
    internal class DateTimeProviderFake(DateTime now) : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return now;
        }
    }
}