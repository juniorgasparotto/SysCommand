using SysCommand.ConsoleApp;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class OnlyMainCommand : Command
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
