using System.Collections.Generic;

namespace SysCommand
{
    public abstract class CommandBase
    {
        public string[] Args { get; internal set; }
        public IEnumerable<CommandMap> Maps { get; internal set; }
        public IEnumerable<CommandParseResult> Result { get; internal set; }

        public object Tag { get; set; }
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }
        public bool UsePrefixInAllMethods { get; set; }
        public string PrefixMethods { get; set; }
        public bool OnlyMethodsWithAttribute { get; set; }
        public bool OnlyPropertiesWithAttribute { get; set; }
        public bool EnablePositionalArgs { get; set; }
    }
}
