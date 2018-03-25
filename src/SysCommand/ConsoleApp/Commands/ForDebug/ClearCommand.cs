using System;

namespace SysCommand.ConsoleApp.Commands
{
    /// <summary>
    /// Used to clear the prompt windows on in debug (use: > clear)
    /// </summary>
    public class ClearCommand : Command
    {
        public ClearCommand()
        {
            HelpText = "Clear window. Only in debug";
            OnlyInDebug = true;
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}