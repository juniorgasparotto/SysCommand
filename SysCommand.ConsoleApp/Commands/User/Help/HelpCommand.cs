namespace SysCommand.ConsoleApp.Commands
{
    public class HelpCommand : Command, IHelpCommand
    {
        public HelpCommand()
        {
            HelpText = "displays help information";
        }

        public string Help(string action = null)
        {
            if (action == null)
                return this.App.Descriptor.GetHelpText(this.App.Maps);
            else
                return this.App.Descriptor.GetHelpText(this.App.Maps, action);
        }
    }
}
