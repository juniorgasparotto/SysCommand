using System;
using SysCommand.ConsoleApp;

namespace Example.Initialization
{
    public class Program
    {
        static void Main(string[] args)
        {
            var app = new App();
            app.Run(args);
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
