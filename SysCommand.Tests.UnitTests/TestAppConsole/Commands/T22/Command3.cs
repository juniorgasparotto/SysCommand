using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T22
{
    public class Command3 : Command
    {
        public int Id { get; set; }
        public void Main()
        {
            App.Console.Write("Id=" + Id);
        }
    }
}
