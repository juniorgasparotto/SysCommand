using System;
namespace SysCommand
{
    [Flags]
    public enum VerboseEnum
    {
        All = 1,
        None = 2,
        Info = 4,
        Success = 8,
        Critical = 16,
        Warning = 32,
        Error = 64,
        Question = 128,
    }
}
