namespace Example
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main()
        {
            return App.RunApplication();
        }
    }

    public class HelloWorld1 : Command
    {
        public string HelloWorld(string myArg0, int? myArg1 = null)
        {
            return string.Format("My HelloWorld (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }
    }

    public class HelloWorld2 : Command
    {
        public string MyArg0 { get; set; }
        public string MyArg1 { get; set; }

        public string Main()
        {
            return string.Format("My console app like MVC  (Arg0: {0}; Arg1: {1})", MyArg0, MyArg1);
        }
    }
}
