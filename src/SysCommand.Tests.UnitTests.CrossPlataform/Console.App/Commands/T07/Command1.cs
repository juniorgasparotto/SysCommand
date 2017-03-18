using SysCommand.ConsoleApp;
using System.Text;

namespace SysCommand.Tests.UnitTests.Commands.T07
{
    public class Command1 : Command
    {
        public StringBuilder Str { get; set; }

        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + ".Main";
        }
    }
}
