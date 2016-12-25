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

    public class HelloWorldWithPropertiesCommand : Command
    {
        public string Arg0 { get; set; }
        public string Arg1 { get; set; }

        public string Main()
        {
            return $"My console app like MVC (Arg0: {Arg0}; Arg1: {Arg1})";
        }
    }

    public class HelloWorldWithMethodsCommand : Command
    {
        public string HelloWorld(string myArg0, double? myArg1 = null)
        {
            return $"My HelloWorld (Arg0: {myArg0}; Arg1: {myArg1})";
        }
    }
}
