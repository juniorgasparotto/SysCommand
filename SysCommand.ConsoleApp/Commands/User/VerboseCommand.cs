using SysCommand.Mapping;

namespace SysCommand.ConsoleApp.Commands
{
    public class VerboseCommand : Command
    {
        [Argument(ShortName = 'v', LongName = "verbose")]
        public Verbose Verbose { get; set; }

        public void Main()
        {
            this.App.Console.Verbose = this.Verbose;
        }
    }
}
