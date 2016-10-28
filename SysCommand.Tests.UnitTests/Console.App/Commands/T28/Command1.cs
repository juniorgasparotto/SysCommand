using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T28
{
    public class Command1 : Command
    {
        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main(int a, int b, int c, int d)
        {
            return $"Main({a}, {b}, {c}, {d})";
        }
    }
}
