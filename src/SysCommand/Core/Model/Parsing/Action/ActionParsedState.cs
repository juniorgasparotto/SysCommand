using System;

namespace SysCommand.Parsing
{
    /// <summary>
    /// Determines the state of an action being parsed.
    /// </summary>
    [Flags]
    public enum ActionParsedState
    {
        /// <summary>
        /// Not parsed
        /// </summary>
        None = 0,
        /// <summary>
        /// When is valid
        /// </summary>
        Valid = 1,
        /// <summary>
        /// When has extras
        /// </summary>
        HasExtras = 2,
        /// <summary>
        /// When no have arguments in the map and in the input
        /// </summary>
        NoArgumentsInMapAndInInput = 4,
        /// <summary>
        /// When is invalid
        /// </summary>
        IsInvalid = 8
    }
}
