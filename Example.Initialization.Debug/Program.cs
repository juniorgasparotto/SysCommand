using System;
using SysCommand.ConsoleApp;

namespace Example.Initialization.Debug
{
    public class Program
    {
        public static int Main()
        {
            return App.RunApplication();
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
