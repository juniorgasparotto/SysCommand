using System;
namespace SysCommand
{
    [Flags]
    public enum VerboseEnum
    {
        All = 0,
        Info = 1,
        Success = 2,
        Critical = 4,
        Warning = 8,
        Error = 16,
        Question = 32,
    }
}
