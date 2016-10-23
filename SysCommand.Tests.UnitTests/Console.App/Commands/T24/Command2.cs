using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T24
{
    public class Command2 : Command
    {
        public bool Help { get; set; }

        public string Main()
        {
            return "Main";
        }
    }
}
