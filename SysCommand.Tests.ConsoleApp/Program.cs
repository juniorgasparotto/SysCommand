using SysCommand;
using SysCommand.ConsoleApp;
using SysCommand.Tests.ConsoleApp.Commands;
using System;
using System.Collections.Generic;

namespace SysCommand.Tests.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            bool lastBreakLineInNextWrite = false;
            while (true)
            {
                var app = new App();

                app.ReadArgsWhenIsDebug = true;
                app.Console.BreakLineInNextWrite = lastBreakLineInNextWrite;
                app.Run();
                lastBreakLineInNextWrite = app.Console.BreakLineInNextWrite;

                if (!App.IsDebug)
                    return app.Console.ExitCode;
            }
        }
    }
}
