using SysCommand.Execution;

namespace SysCommand
{
    /// <summary>
    /// Inherit this class to create your command.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// The current execution scope
        /// </summary>
        public ExecutionScope ExecutionScope { get; set; }

        /// <summary>
        /// If true, this command will only be loaded in debug mode.
        /// </summary>
        public bool OnlyInDebug { get; set; }

        /// <summary>
        /// Determines whether this command use prefix for methods
        /// </summary>
        public bool UsePrefixInAllMethods { get; set; }

        /// <summary>
        /// Prefix for all methods if enabled
        /// </summary>
        public string PrefixMethods { get; set; }

        /// <summary>
        /// If true, only methods with "ActionAttribute" is loaded
        /// </summary>
        public bool OnlyMethodsWithAttribute { get; set; }

        /// <summary>
        /// If true, only arguments with "ArgumentAttribute" is loaded
        /// </summary>
        public bool OnlyPropertiesWithAttribute { get; set; }

        /// <summary>
        /// Determines whether properties can use positional arguments.
        /// </summary>
        public bool EnablePositionalArgs { get; set; }

        /// <summary>
        /// Help text
        /// </summary>
        public string HelpText { get; set; }
    }
}
