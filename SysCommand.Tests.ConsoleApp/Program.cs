using SysCommand.ConsoleApp;

namespace SysCommand.Tests.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            return App.RunInfiniteIfDebug();
        }
    }
}