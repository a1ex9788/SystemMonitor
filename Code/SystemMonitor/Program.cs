using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using SystemMonitor.Logic;

namespace SystemMonitor
{
    internal class Program
    {
        internal static int Main(string[] args)
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
            catch (Exception e)
            {
                Console.Error.WriteLine("An unexpected error was produced.");

                new ProgramErrorsLogger().LogError(e);

                return -1;
            }
        }

        private static void DefineMonitorCommand(CommandLineApplication commandLineApplication)
        {
            CommandOption directoryCommandOption = commandLineApplication.Option(
                "-d",
                "The directory to monitor. The whole file system is monitored if it is not specified.",
                CommandOptionType.SingleValue);

            commandLineApplication.OnExecuteAsync(ct =>
            {
                IServiceProvider serviceProvider = new MonitorCommandServiceProvider(ct);

                IMonitorCommand monitorCommand = serviceProvider
                    .GetRequiredService<IMonitorCommand>();

                return monitorCommand.ExecuteAsync(directoryCommandOption.Value());
            });
        }
    }
}