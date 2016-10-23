using System;

namespace SysCommand.Parsing
{
    [Flags]
    public enum ActionParsedState
    {
        None = 0,
        Valid = 1,
        HasExtras = 2,
        NoArgumentsInMapAndInInput = 4,
        IsInvalid = 8
    }
}
