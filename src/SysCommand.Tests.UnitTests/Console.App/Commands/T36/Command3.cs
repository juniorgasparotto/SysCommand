using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using System.Collections.Generic;
namespace SysCommand.Tests.UnitTests.Commands.T36
{
    public class Command3 : Command
    {
        [Argument(ShortName = 'a', LongName = "a1")]
        public List<string> A1 { get; set; }

        public Command3()
        {
            EnablePositionalArgs = true;
        }

    }
}
