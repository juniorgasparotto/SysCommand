using Fclp;
using System;

namespace SysCommand
{
    public class ManageArgsHistoryCommand : CommandBase, IManageArgsHistoryCommand
    {
        public string CmdName { get; set; }
        public string CmdSave { get; set; }
        public string CmdDelete { get; set; }

        public ManageArgsHistoryCommand()
        {
            this.Tag = CommandTag.System;
        }

        public string[] Main(string[] args)
        {
            return args;
        }
    }
}
