using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Commands.T38
{
    public class Command1 : Command
    {
        [Argument(ShortName = 'a', LongName = "a1", IsRequired = true, DefaultValue = "command1")]
        public string A1 { get; set; }

        public Command1()
        {
            EnablePositionalArgs = true;
        }
    }
}
