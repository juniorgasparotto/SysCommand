namespace SysCommand.ConsoleApp.Results
{
    public class RedirectResult : IActionResult
    {
        public string[] NewArgs { get; private set; }

        public RedirectResult(params string[] newArgs)
        {
            this.NewArgs = newArgs;
        }
    }
}