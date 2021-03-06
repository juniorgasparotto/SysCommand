﻿using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Common.Commands.T30
{
    public class Command1 : Command
    {
        [Argument(Help ="Prop1 without show complement", ShowHelpComplement = false, DefaultValue ="test")]
        public string Prop1 { get; set; }

        [Argument(Help = "Prop2", ShowHelpComplement = true)]
        public decimal Prop2 { get; set; }
    }
}
