using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T09
{
    public class Command1 : Command
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }

        public Command1()
        {
            this.EnablePositionalArgs = true;
        }
    }
}
