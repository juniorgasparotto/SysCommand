using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Common.Commands.T30
{
    public class Command2 : Command
    {
        public enum Enum1
        {
            Y,
            N
        }

        public enum Enum2
        {
            Value1 = 10,
            Value2 = 20
        }

        [Argument(Help = "Prop1 without show complement", ShowHelpComplement = false, DefaultValue = "test")]
        public string Prop1 { get; set; }

        [Argument( IsRequired = true, Help = "Loren ipsulum Loren ipsulum Loren ipsulum Loren ipsulum Loren ipsulum Loren ipsulum ", ShowHelpComplement = true)]
        public decimal Prop2 { get; set; }

        public Command2()
        {
            this.HelpText = "Loren ipsulum Loren ipsulum Loren ipsulum Loren Loren ipsulum Loren ipsulum Loren ipsulum Loren Loren ipsulum Loren ipsulum Loren ipsulum LorenLoren ipsulum Loren ipsulum Loren ipsulum Loren";
        }

        [Action(Help = "Loren ipsulum Loren ipsulum Loren ipsulum Loren")]
        public void Save(string value = "test", List<int> lst = null, Enum1 enum1 = Enum1.Y)
        {

        }

        [Action(Help = "Loren ipsulum Loren ipsulum Loren ipsulum Loren")]
        public void Save(string value, List<int> lst = null, Enum2 enum2 = Enum2.Value2)
        {

        }
    }
}
