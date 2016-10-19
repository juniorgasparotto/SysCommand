using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T23
{
    public class Command1 : Command
    {
        //[Argument(IsRequired=true)]
        //public string Id { get; set; }

        public string Main()
        {
            return "Main";
        }

        public string Save(string a, string id)
        {
            return "Save: a=" + a + ", id=" + id;
        }
    }
}
