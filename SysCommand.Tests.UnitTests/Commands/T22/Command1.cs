using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T22
{
    public class Command1 : Command
    {
        [Argument(IsRequired=true)]
        public string Id { get; set; }

        public decimal Price { get; set; }

        public string Main()
        {
            return "Price = " + Price + "; Id = " + Id;
        }
    }
}
