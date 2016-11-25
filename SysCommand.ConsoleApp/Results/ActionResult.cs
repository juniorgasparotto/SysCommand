namespace SysCommand.ConsoleApp.Results
{
    public class ActionResult : IActionResult
    {
        public object Value { get; private set; }


        public ActionResult(object value)
        {
            this.Value = value;
        }
    }
}