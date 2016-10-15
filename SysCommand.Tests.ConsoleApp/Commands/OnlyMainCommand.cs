using SysCommand.ConsoleApp;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class OnlyMainCommand : SysCommand.ConsoleApp.Command
    {
        public char a { get; set; }

        public string Main()
        {
            return "Main";
        }

        //[Action(IsDefault=true)]
        //public string Default()
        //{
        //    return "Default";
        //}
    }
}
