using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Common.Commands.T40
{
    public class Command1 : Command
    {
        public string Prop1
        {
            set
            {
                App.Console.Write(GetArgument("Prop1").Name);
            }
        }

        public Command1()
        {
            this.App.Console.Write("Constructor");
        }

        public Command1(bool ignoreApp)
        {
            
        }

        public string TestActionMap(int A)
        {
            var a = GetActionMap();
            return a.ActionName;
        }

        public string TestAction(int A)
        {
            var a = GetAction();
            return a.Name;
        }
    }
}
