using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T20
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
