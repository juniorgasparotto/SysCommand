namespace MyNamespace.Commands
{
    using SysCommand.ConsoleApp;

    public class MyCommand : Command
    {
        public string MyProperty { get; set; }

        public string Main()
        {
            return $"MyProperty: {MyProperty}";
        }

        public string MyAction(int? parameter = null)
        {
            return $"parameter: {parameter}";
        }
    }
}