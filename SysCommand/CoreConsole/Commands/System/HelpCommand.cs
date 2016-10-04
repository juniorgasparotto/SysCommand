using System;

namespace SysCommand.ConsoleApp
{
    public class ManageArgsHistoryCommand : CommandBase, IHelpCommand
    {
        public Application App { get; internal set; }

        public void Help()
        {
            throw new NotImplementedException();
        }

        public void Help(char m)
        {
            throw new NotImplementedException();
        }
    }
}
