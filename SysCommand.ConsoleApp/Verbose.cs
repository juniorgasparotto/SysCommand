using System;
namespace SysCommand.ConsoleApp
{
    [Flags]
    public enum Verbose
    {
        None = 0,
        All = 1,
        Info = 2,
        Success = 4,
        Critical = 8,
        Warning = 16,
        Error = 32,
        Quiet = 64
    }
}
