﻿using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T39
{
    public class Command1 : Command
    {
        [Action(Help = "Loren ipsulum Loren ipsulum Loren ipsulum Loren")]
        public void Save(int A)
        {

        }
    }
}