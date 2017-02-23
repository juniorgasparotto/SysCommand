using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Results;

namespace Example.Commands
{
    public class RedirectCommand : Command
    {
        private int _count;
        
        public RedirectResult RedirectNow(string arg)
        {
            _count++;
            App.Console.Write($"Redirecting now!!. Count: {_count}");
            return new RedirectResult("redirected", "--arg", arg);
        }

        public string Something()
        {
            return "Something";
        }

        public string Redirected(string arg)
        {
            _count++;
            return $"Redirected: {arg}. Count: {_count}";
        }
    }

}
