/// usage:
/// 1) Start debugging
/// 2) Enter with commands:
///     > help
///     > my-action
///     > my-action 10
///     > my-action --parameter 10
///     > --my-property "abc"
///     > my-action 10 --my-property "abc"
///     > main --parameter1 "a" --parameter2 "b"
///     > --parameter1 "a" --parameter2 "b"
namespace MyNamespace
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main()
        {
            return App.RunApplication();
        }
    }
}