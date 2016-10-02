namespace SysCommand
{
    public abstract class Command
    {
        public App App { get; set; }
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }
        public bool UsePrefixInAllMethods { get; set; }
        public string PrefixMethods { get; set; }
        public bool OnlyMethodsWithAttribute { get; set; }
        public bool OnlyPropertiesWithAttribute { get; set; }
        public bool EnablePositionalArgs { get; set; }
    }
}
