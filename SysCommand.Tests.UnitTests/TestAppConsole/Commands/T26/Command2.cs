using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T26
{
    public class Command2 : Command
    {
        [Argument(DefaultValue = 100)]
        public int Id { get; set; }

        public string Main()
        {
            return "Main= Id:" + Id;
        }
    }
}
