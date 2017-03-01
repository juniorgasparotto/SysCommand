namespace MyNamespace.Commands
{
    using SysCommand.ConsoleApp;

    public class MyOtherCommand : Command
    {
        public string Main(string parameter1, string parameter2 = null)
        {
            return $"parameter1: {parameter1}; parameter2: {parameter2}";
        }
    }
}