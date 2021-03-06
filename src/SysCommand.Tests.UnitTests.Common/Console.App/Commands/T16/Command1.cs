﻿using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Common.Commands.T16
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
