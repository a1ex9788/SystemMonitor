using System;

namespace SystemMonitor.Exceptions
{
    internal abstract class SystemMonitorException(string message) : Exception(message)
    {
    }
}