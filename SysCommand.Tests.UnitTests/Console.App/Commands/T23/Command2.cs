using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T23
{
    public class Command2 : Command
    {
        public string Help { get; set; }

        public string Main()
        {
            return "Main";
        }

        public string Save(string id)
        {
            return "Save: id=" + id;
        }
    }
}
