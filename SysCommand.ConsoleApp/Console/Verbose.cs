using System;
namespace SysCommand.ConsoleApp
{
    [Flags]
    public enum Verbose
    {
        All = 0,
        None = 1,
        Info = 2,
        Success = 4,
        Critical = 8,
        Warning = 16,
        Error = 32
    }
}
