using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Commands.T20
{
    public class Command2 : Command
    {
        public char a { get; set; }

        public string Main()
        {
            return "Main";
        }
    }
}
