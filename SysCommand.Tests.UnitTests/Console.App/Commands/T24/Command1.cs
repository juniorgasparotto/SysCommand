using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T24
{
    public class Command1 : Command
    {
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
