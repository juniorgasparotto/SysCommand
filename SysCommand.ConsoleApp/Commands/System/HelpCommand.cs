using System;

namespace SysCommand.ConsoleApp
{
    public class HelpCommand : Command, IHelpCommand
    {
        public void Help()
        {
            this.App.Console.Write("Help()");
        }

        public void Help(char m)
        {
            this.App.Console.Write("Help(char m) = " + m);
        }
    }
}
