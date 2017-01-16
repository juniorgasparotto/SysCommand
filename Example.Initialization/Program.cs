using SysCommand.ConsoleApp;

namespace Example.Initialization
{
    public class Program
    {
        //public static int Main(string[] args)
        //{
        //    return App.RunApplication(args);
        //}

        public static int Main(string[] args)
        {
            var app = new App();
            app.Run(args);
            return app.Console.ExitCode;
        }
        
    }
}
