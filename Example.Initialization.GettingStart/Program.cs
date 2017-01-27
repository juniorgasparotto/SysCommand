namespace Example.Initialization.GettingStart
{
    using SysCommand.ConsoleApp;
    using SysCommand.Mapping;
    using System;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();

            var myApp = new App();
            myApp.Run(args);
            return myApp.Console.ExitCode;
        }
    }

    public class MyCommand : Command
    {
        public string MyPropertyString { get; set; }

        [Argument(LongName="property", ShortName='p', Help="My property help")]
        public int? MyPropertyInt { get; set; }

        public void Main()
        {
            if (MyPropertyString != null)
            {
                // using in your prompt to use this argument: 
                // MyApp.exe --my-property-string value
                Console.WriteLine("MyPropertyString");
            }

            if (MyPropertyInt != null)
            {
                // using in your prompt to use this argument: 
                // MyApp.exe --property 123
                // MyApp.exe -p 123
                Console.WriteLine("MyPropertyInt");
            }
        }

        public string MyAction(string myParameter)
        {
            // using in your prompt to use this action: 
            // MyApp.exe my-action --my-parameter value
            return "MyAction";
        }

        [Action(Name="custom-action", Help = "Action help")]
        public string MyCustomAction
        (
            [Argument(ShortName = 'a')]
            decimal myParameter)
        {
            // using in your prompt to use this action:
            // MyApp.exe custom-action -a 9999.99
            return "MyCustomAction";
        }
    }
}