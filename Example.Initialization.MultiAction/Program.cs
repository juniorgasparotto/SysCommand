using SysCommand.ConsoleApp;

namespace Example.Initialization.MultiAction
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new App().Run(args);
        }

        public class MyCommand : Command
        {
            public string Action1(string value = "default")
            {
                return $"Action1 (value = {value})";
            }

            public string Action2(string value = "default")
            {
                return $"Action2 (value = {value})";
            }
        }
    }
}
