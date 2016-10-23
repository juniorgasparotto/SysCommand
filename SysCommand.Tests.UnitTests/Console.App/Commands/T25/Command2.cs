using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T25
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
