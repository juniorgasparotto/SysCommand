using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T13
{
    public class Command3 : Command
    {
        public string Description { get; set; }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main()");
        }
    }
}
