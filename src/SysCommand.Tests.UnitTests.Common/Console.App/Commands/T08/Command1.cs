using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T08
{
    public class Command1 : Command
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }

        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main() = {0}, {1}, {2}", A, B, C);
        }
    }
}
