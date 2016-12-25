using SysCommand.Mapping;

namespace SysCommand.ConsoleApp.Commands
{
    public class VerboseCommand : Command
    {
        [Argument(ShortName = 'v', LongName = "verbose")]
        public Verbose Verbose { get; set; }

        public VerboseCommand()
        {
            this.HelpText = "Enables and disables verbose mode";
        }

        public void Main()
        {
            this.App.Console.Verbose = this.Verbose;
        }
    }
}
