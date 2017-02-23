namespace SysCommand.ConsoleApp.Helpers
{
    public static class DebugHelper
    {
        public static bool IsDebug { get { return System.Diagnostics.Debugger.IsAttached; } }
    }
}
