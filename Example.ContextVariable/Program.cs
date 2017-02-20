namespace Example.Initialization.Advanced
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication(delegate()
            {
                var app = new App();
                app.Items["variable1"] = 1;
                return app;
            });
        }
    }

    public class Command1 : Command
    {
        public void Action()
        {
            this.App.Console.Write(App.Items["variable1"]);
            App.Items["variable1"] = (int)App.Items["variable1"] + 1;
        }

        public void Action2()
        {
            this.App.Console.Write(App.Items["variable1"]);
        }
    }
}