using SysCommand.ConsoleApp;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class MainCommand : Command
    {
        public string Main()
        {
            return "Main";
        }

        public string main()
        {
            return "main";
        }
    }
}
