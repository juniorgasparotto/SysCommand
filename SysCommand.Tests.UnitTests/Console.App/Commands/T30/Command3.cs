using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T30
{
    public class Command3 : Command
    {
        [Action(Help = "Loren ipsulum Loren ipsulum Loren ipsulum Loren")]
        public void Save()
        {

        }
    }
}
