﻿using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T34
{
    public class Command3 : Command
    {
        [Argument(ShortName = 'b', LongName = "b1")]
        public string B2 { get; set; }
    }
}
