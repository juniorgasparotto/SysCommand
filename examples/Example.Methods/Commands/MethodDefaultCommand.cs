using System.Linq;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace Example.Commands
{
    public class MethodDefaultCommand : Command
    {
        public string Main(string arg0)
        {
            return "Main(string arg0)";
        }

        public string Main(string arg0, string arg1)
        {
            return "Main(string arg0, string arg1)";
        }

        [Action(IsDefault = false)]
        public string Main(int argument)
        {
            return "Main(int argument)";
        }

        [Action(IsDefault = true)]
        public string AnyName(string argument)
        {
            return "AnyName(string argument)";
        }

        [Action(IsDefault = true)]
        public string ActionWhenNotExistsInput()
        {
            return "ActionWhenNotExistsInput()";
        }
    }
}