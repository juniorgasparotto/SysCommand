using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Common.Commands.T38
{
    public class Command2 : Command
    {
        [Argument(ShortName = 'a', LongName = "a1", IsRequired = true, DefaultValue = "command2")]
        public string A1 { get; set; }

        public Command2()
        {
            EnablePositionalArgs = true;
        }

    }
}
