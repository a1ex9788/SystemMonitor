using System.IO;
using SystemMonitor.Logic.Utilities.DateTimes;

namespace SystemMonitor.Logic
{
    public class OutputDirectory
    {
        public OutputDirectory(IDateTimeProvider dateTimeProvider)
        {
            string formattedData = dateTimeProvider.GetCurrentDateTime().ToDirectoryName();

            this.Path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), formattedData);
        }

        public string Path { get; }
    }
}