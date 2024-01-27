using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;

namespace SystemMonitor
{
    internal class ProgramErrorsLogger
    {
        private static readonly string ErrorsFile = "Errors.txt";

        private readonly IFile file;

        internal ProgramErrorsLogger()
        {
            IServiceProvider serviceProvider = new MonitorCommandServiceProvider();

            this.file = serviceProvider.GetRequiredService<IFile>();
        }

        internal void LogError(Exception exception)
        {
            this.file.WriteAllText(ErrorsFile, exception.ToString());
        }
    }
}