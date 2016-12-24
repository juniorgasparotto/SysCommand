using SysCommand.Mapping;

namespace SysCommand.ConsoleApp.Commands
{
    public class VerboseCommand : Command
    {
        [Argument(ShortName = 'v', LongName = "verbose")]
        public Verbose Verbose { get; set; }

        public VerboseCommand()
        {
            //this.OrderExecution = -1;
        }

        public void Main()
        {
            this.App.Console.Verbose = this.Verbose;
        }
    }
}
