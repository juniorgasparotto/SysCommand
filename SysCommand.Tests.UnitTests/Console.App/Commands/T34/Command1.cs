using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T34
{
    public class Command1 : Command
    {
        [Argument(ShortName = 'a', LongName = "a1")]
        public string A1 { get; set; }
    }
}
