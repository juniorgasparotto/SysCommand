using SysCommand.ConsoleApp;

namespace SysCommand.Tests
{
    public class TestVerboseCommand : Command
    {
        public TestVerboseCommand()
        {
            this.OrderExecution = -1;
        }

        public void TestVerbose()
        {
            App.Console.Info(string.Format("Info '{0}'", (int)Verbose.Info));
            App.Console.Success(string.Format("Success '{0}'", (int)Verbose.Success));
            App.Console.Critical(string.Format("Critical '{0}'", (int)Verbose.Critical));
            App.Console.Warning(string.Format("Warning '{0}'", (int)Verbose.Warning));
            App.Console.Error(string.Format("Error '{0}'", (int)Verbose.Error));
        }
    }
}
