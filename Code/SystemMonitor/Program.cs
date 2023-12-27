using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using SystemMonitor.Logic;

namespace SystemMonitor
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                using CommandLineApplication commandLineApplication = new CommandLineApplication()
                {
                    Name = "systemMonitor",
                };

                commandLineApplication.HelpOption();

                DefineMonitorCommand(commandLineApplication);

                return commandLineApplication.Execute(args);
            }
            catch
            {
                Console.Error.WriteLine("An unexpected error was produced.");

                return -1;
            }
        }

        private static void DefineMonitorCommand(CommandLineApplication commandLineApplication)
        {
            commandLineApplication.OnExecuteAsync(async ct =>
            {
                IServiceProvider serviceProvider = new MonitorCommandServiceProvider(ct);

                IMonitorCommand monitorCommand = serviceProvider
                    .GetRequiredService<IMonitorCommand>();

                await monitorCommand.ExecuteAsync();
            });
        }
    }
}