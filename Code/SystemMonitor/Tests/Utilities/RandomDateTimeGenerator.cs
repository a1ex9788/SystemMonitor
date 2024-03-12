using System;

namespace SystemMonitor.Tests.Utilities
{
    internal static class RandomDateTimeGenerator
    {
        private static readonly Random random = new Random();

        public static DateTime Get()
        {
            return DateTime.MinValue.AddDays(random.Next(0, 1000000));
        }
    }
}