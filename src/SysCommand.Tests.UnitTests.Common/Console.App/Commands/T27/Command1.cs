using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Commands.T27
{
    public class Command1 : Command
    {
        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main(int a, int b)
        {
            return $"Main({a}, {b})";
        }

        public string Main(string[] a)
        {
            return $"Main(string[] a)";
        }
    }
}
