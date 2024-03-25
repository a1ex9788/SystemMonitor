using System.IO.Abstractions;
using SystemMonitor.Logic.DateTimes;

namespace SystemMonitor.Logic.Output.Factory
{
    internal class OutputWriterFactory : IOutputWriterFactory
    {
        public IOutputWriter Create(
            IDateTimeProvider dateTimeProvider, IDirectory directory, IFile file, OutputFilesInfo outputFilesInfo)
        {
            return new OutputWriter(dateTimeProvider, directory, file, outputFilesInfo);
        }
    }
}