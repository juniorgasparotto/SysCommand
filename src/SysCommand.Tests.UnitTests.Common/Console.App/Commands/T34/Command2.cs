using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Common.Commands.T34
{
    public class Command2 : Command
    {
        [Argument(ShortName = 'b', LongName = "b1")]
        public string B2 { get; set; }
    }
}
