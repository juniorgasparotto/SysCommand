using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using System.Collections.Generic;
namespace SysCommand.Tests.UnitTests.Common.Commands.T37
{
    public class Command2 : Command
    {
        [Argument(ShortName = 'a', LongName = "a1")]
        public List<string> A1 { get; set; }

        public Command2()
        {
            EnablePositionalArgs = true;
        }

    }
}
