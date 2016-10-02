using SysCommand;
using SysCommand.Tests.ConsoleApp.Commands;
using System.Collections.Generic;

namespace SysCommand.Tests.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var result = new App2(null, new List<Command>() { new MainCommand() }).Run();

            var commandLoader = new DefaultCommandLoader();
            var commands = commandLoader.GetFromAppDomain(ConsoleHelper.IsDebug);
            var app2 = new Executor(ConsoleHelper.GetArguments(), commands);
            app2.Execute();
        }
    }
}
