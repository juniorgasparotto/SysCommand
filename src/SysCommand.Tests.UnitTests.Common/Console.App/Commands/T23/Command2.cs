using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T23
{
    public class Command2 : Command
    {
        public string Help { get; set; }

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
