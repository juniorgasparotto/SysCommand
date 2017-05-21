namespace SysCommand.ConsoleApp.Commands
{
    /// <summary>
    /// The help command (user: > help)
    /// </summary>
    public class HelpCommand : Command, IHelpCommand
    {
        public HelpCommand()
        {
            HelpText = "Displays help information";
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
