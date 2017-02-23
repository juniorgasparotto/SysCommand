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
            if (this.Verbose.HasFlag(Verbose.Quiet))
                this.App.Console.Verbose = Verbose.None;
            else
                this.App.Console.Verbose = Verbose.Info | this.Verbose;
        }
    }
}
