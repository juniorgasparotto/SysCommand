using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Commands.T22
{
    public class Command1 : Command
    {
        public string a { get; set; }

        public string Main()
        {
            return "Main";
        }
    }
}
