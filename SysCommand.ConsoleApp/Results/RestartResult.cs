namespace SysCommand.ConsoleApp.Results
{
    public class RestartResult : IActionResult
    {
        public string[] NewArgs { get; private set; }

        public RestartResult(string[] newArgs)
        {
            this.NewArgs = newArgs;
        }
    }
}