using SysCommand.ConsoleApp;
using SysCommand.Execution;
using SysCommand.Mapping;
using System;
using System.Collections.Generic;

namespace SysCommand.Tests.UnitTests.Commands.T36
{
    public class Command1 : Command
    {
        [Argument(ShortName = 'a', LongName = "a1")]
        public string A1 { get; set; }

        [Argument(ShortName = 'b', LongName = "b1")]
        public List<string> A2 { get; set; }

        public Command1()
        {
            EnablePositionalArgs = true;
        }
    }
}
