using SysCommand.ConsoleApp;

namespace SysCommand.Tests.UnitTests.Common.Commands.T23
{
    public class Command1 : Command
    {
        public bool Help { get; set; }

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
