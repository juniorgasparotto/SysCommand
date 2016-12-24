using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T32
{
    public class Command3 : Command
    {
        [Argument(Help = "Prop1 without show complement", ShowHelpComplement = false)]
        public string Prop1 { get; set; }

        public Command3()
        {
            this.EnablePositionalArgs = true;
        }
    }
}
