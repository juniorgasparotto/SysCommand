using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Commands;
using SysCommand.ConsoleApp.Loader;

namespace Example.Initialization.Exclusive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create loader instance
            var loader = new AppDomainCommandLoader();

            // Remove unwanted command
            loader.IgnoreCommand<FirstCommand>();
            loader.IgnoreCommand<VerboseCommand>();
            loader.IgnoreCommand<ArgsHistoryCommand>();

            // Get all commands with 'ignored' filter
            var commandsTypes = loader.GetFromAppDomain();

            new App(commandsTypes).Run(args);
        }

        public class FirstCommand : Command
        {
            public string FirstProperty
            {
                set
                {
                    App.Console.Write("FirstProperty");
                }
            }
        }

        public class SecondCommand : Command
        {
            public string SecondProperty
            {
                set
                {
                    App.Console.Write("SecondProperty");
                }
            }
        }
    }
}
