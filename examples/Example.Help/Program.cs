using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace Example.Help
{
    class Program
    {
        public static int Main()
        {
            return App.RunApplication();
        }

        //public class CustomHelp : Command, SysCommand.ConsoleApp.Commands.IHelpCommand
        //{
        //    public string MyCustomHelp(string action = null)
        //    {
        //        foreach (var map in this.App.Maps)
        //        {

        //        }

        //        return "Custom help";
        //    }
        //}

        public class HelpCommand : Command
        {
            // With complement
            [Argument(Help = "My property1 help")]
            public string MyProperty1 { get; set; }

            // Without complement
            [Argument(Help = "My property2 help", ShowHelpComplement = false, IsRequired = true)]
            public int MyProperty2 { get; set; }

            public HelpCommand()
            {
                this.HelpText = "Help for this command";
            }

            [Action(Help = "Action help")]
            public void MyActionHelp
            (
                [Argument(Help = "Argument help")]
                string arg0, // With complement

                [Argument(Help = "Argument help", ShowHelpComplement = false)]
                string arg1  // Without complement
            )
            {
            }
        }
    }
}
