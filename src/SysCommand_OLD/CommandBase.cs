using SysCommand.Execution;

namespace SysCommand
{
    public abstract class CommandBase
    {
        public ExecutionScope ExecutionScope { get; set; }
        public bool OnlyInDebug { get; set; }
        public bool UsePrefixInAllMethods { get; set; }
        public string PrefixMethods { get; set; }
        public bool OnlyMethodsWithAttribute { get; set; }
        public bool OnlyPropertiesWithAttribute { get; set; }
        public bool EnablePositionalArgs { get; set; }
        public string HelpText { get; set; }
    }
}
