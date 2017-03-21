using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T24
{
    public class Command2 : Command
    {
        public bool Help { get; set; }

        public string Main()
        {
            return "Main";
        }
    }
}
