using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;

namespace SystemMonitor
{
    internal class ProgramErrorsLogger
    {
        // TODO: Use Directory.GetCurrentDirectory().
        private static readonly string ErrorsFile = "Errors.txt";

        private readonly IFile file;

        public ProgramErrorsLogger()
        {
            IServiceProvider serviceProvider = new MonitorCommandServiceProvider();

            this.file = serviceProvider.GetRequiredService<IFile>();
        }

        public void LogError(Exception exception)
        {
            this.file.WriteAllText(ErrorsFile, exception.ToString());
        }
    }
}