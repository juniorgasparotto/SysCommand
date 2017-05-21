namespace SysCommand.ConsoleApp.Results
{
    /// <summary>
    /// Use this class to redirect your application with a new sequence of commands, 
    /// just to your action return an instance of the class RedirectResult by passing in your 
    /// constructor a string containing the new string of commands. It is worth mentioning that 
    /// the instances of the controls will be the same, that is, the State of each command will 
    /// not return to the start, just the flow of execution. Another important point is that any
    /// action after action that returned the RedirectResult will no longer be called.
    /// </summary>
    public class RedirectResult : IActionResult
    {
        /// <summary>
        /// New arguments to re-start
        /// </summary>
        public string[] NewArgs { get; private set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="newArgs">New arguments to re-start</param>
        public RedirectResult(params string[] newArgs)
        {
            this.NewArgs = newArgs;
        }
    }
}