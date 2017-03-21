using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T24
{
    public class Command1 : Command
    {
        public string Main()
        {
            return "Main";
        }

        public string Save(string id)
        {
            return "Save: id=" + id;
        }
    }
}
