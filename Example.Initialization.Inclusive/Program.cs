using SysCommand.ConsoleApp;

namespace Example.Initialization.Inclusive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var commandsTypes = new[]
            {
                typeof(FirstCommand)
            };

            // Specify what you want.
            new App(commandsTypes).Run(args); 

            // Search for any class that extends from Command.
            /*
            new App().Run(args);
            */
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
