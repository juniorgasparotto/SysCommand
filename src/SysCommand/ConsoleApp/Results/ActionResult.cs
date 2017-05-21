namespace SysCommand.ConsoleApp.Results
{
    /// <summary>
    /// Represents a result of an action
    /// </summary>
    public class ActionResult : IActionResult
    {
        /// <summary>
        /// Action value 
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="value">Action value</param>
        public ActionResult(object value)
        {
            this.Value = value;
        }
    }
}