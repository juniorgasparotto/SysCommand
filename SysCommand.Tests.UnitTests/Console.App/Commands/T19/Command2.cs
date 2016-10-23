using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T19
{
    public class Command2 : Command
    {
        public void Main()
        {
            App.Console.Write("Main");
        }
    }
}
