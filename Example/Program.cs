using SysCommand.ConsoleApp;

namespace Example
{
    public class Program
    {
        public static int Main()
        {
            return App.RunInfiniteIfDebug();
        }
    }

    public class Command1 : Command
    {
        public string HelloWorld(string arg0 = null, string arg1 = null)
        {
            return "My first console app using MVC";
        }
    }
}
