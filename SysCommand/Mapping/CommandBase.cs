using SysCommand.Evaluation;

namespace SysCommand.Mapping
{
    public abstract class CommandBase
    {
        public EvaluateScope EvaluateScope { get; set; }
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
