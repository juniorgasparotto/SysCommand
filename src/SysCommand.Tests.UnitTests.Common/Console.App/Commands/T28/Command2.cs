using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T28
{
    public class Command2 : Command
    {
        public int C { get; set; }
        public int D { get; set; }

        public Command2()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main(int a, int b)
        {
            return "Main= a:" + a + "; b:" + b;
        }
    }
}
