using SysCommand.ConsoleApp;

namespace Example.Initialization.Static
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //App.RunApplication(() =>
            //{
            //    // customizations
            //    var app = new App(enableMultiAction: false);
            //    return app;
            //});
            App.RunApplication();
        }

        public class MyCommand : Command
        {
            public string MyProperty
            {
                set
                {
                    App.Console.Write(value);
                }
            }
        }
    }
}
