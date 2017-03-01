using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Files;

namespace SysCommand.Tests.ConsoleApp.Commands
{
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
        public string Main(string parameter1, string parameter2 = null)
        {
            return $"parameter1: {parameter1}; parameter2: {parameter2}";
        }
    }
}
