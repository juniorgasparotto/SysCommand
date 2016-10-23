using System;

namespace SysCommand.Parsing
{
    [Flags]
    public enum ArgumentMappingState
    {
        None = 0,
        ArgumentAlreadyBeenSet = 1,
        ArgumentNotExistsByName = 2,
        ArgumentNotExistsByValue = 4,
        ArgumentIsRequired = 8,
        ArgumentHasInvalidInput = 16,
        ArgumentHasUnsupportedType = 32,
        IsInvalid = 64,
        Valid = 128,
        ArgumentIsNotRequired = 256
    }
}
