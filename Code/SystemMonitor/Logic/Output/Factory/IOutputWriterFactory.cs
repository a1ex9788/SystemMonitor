using System.IO.Abstractions;
using SystemMonitor.Logic.DateTimes;

namespace SystemMonitor.Logic.Output.Factory
{
    internal interface IOutputWriterFactory
    {
        IOutputWriter Create(
            IDateTimeProvider dateTimeProvider, IDirectory directory, IFile file, OutputFilesInfo outputFilesInfo);
    }
}