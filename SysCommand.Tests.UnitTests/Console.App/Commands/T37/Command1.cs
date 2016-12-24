using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T37
{
    public class Command1 : Command
    {
        [Argument(ShortName = 'a', LongName = "a1")]
        public string A1 { get; set; }

        public Command1()
        {
            EnablePositionalArgs = true;
        }
    }
}
