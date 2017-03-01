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
    using SysCommand.ConsoleApp.Files;
    using SysCommand.Mapping;

    public class Program
    {
        public static int Main()
        {
            var code = App.RunApplication(breakEndLine: true);
            return code;
        }
    }

    public class MyCommand : Command
    {
        public string MyProperty { get; set; }

        public string Main()
        {
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var cmd = fileManager.GetOrCreate<MyCommand>();
            fileManager.Save(cmd);
            return $"MyProperty: {MyProperty}";
        }

        public string MyAction(int? parameter = null)
        {
            return $"parameter: {parameter}";
        }
    }

    public class MyOtherCommand : Command
    {
        public string Main(string myProperty, int b)
        {
            return "Main(string myProperty, int b)";
        }

        public string Main(string parameter1, string parameter2 = null)
        {
            return $"parameter1: {parameter1}; parameter2: {parameter2}";
        }
    }
}