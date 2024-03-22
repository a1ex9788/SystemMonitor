using System;
using System.Globalization;

namespace SystemMonitor.Logic.DateTimes
{
    internal static class DateTimeExtensions
    {
        public static string ToDirectoryName(this DateTime dateTime)
        {
            return dateTime
                .ToString(CultureInfo.InvariantCulture)
                .Replace('/', '-')
                .Replace(':', '_');
        }
    }
}