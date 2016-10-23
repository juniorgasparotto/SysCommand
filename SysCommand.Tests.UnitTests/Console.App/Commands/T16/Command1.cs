using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T16
{
    public class Command1 : Command
    {
        [Argument(IsRequired=true)]
        public int Id { get; set; }

        public decimal Price { get; set; }

        public void Main()
        {
            App.Console.Write("Price=" + Price + "; Id=" + Id);
        }
    }
}
