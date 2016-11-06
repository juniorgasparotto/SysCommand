namespace SysCommand.ConsoleApp.Commands
{
    public class ManageArgsHistoryCommand : Command, IManageArgsHistoryCommand
    {
        public string CmdName { get; set; }
        public string CmdSave { get; set; }
        public string CmdDelete { get; set; }

        public string[] Main(string[] args)
        {
            return args;
        }
    }
}
