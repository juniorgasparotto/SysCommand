using SysCommand;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class MainCommand : CommandBase
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
