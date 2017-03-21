using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T28
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
