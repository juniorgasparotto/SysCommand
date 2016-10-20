using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T13
{
    public class Command3 : Command
    {
        public string Description { get; set; }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main()");
        }
    }
}
