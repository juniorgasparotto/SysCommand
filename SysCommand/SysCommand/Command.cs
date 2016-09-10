namespace SysCommand
{
    public abstract class Command
    {
        //private List<ActionMap> actionsMaps = new List<ActionMap>();
        //private List<ArgumentMap> argumentsMaps = new List<ArgumentMap>();

        public App App { get; set; }
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }
        public bool UsePrefixInAllMethods { get; set; }
        public string PrefixMethods { get; set; }
        public bool OnlyMethodsWithAttribute { get; set; }
        public bool OnlyPropertiesWithAttribute { get; set; }
        public bool EnablePositionalArgs { get; set; }

        public Command(App app)
        {
            this.App = app;
        }

        public abstract void Main();

        //public List<ActionMap> ActionsMaps
        //{
        //    get
        //    {
        //        if (this.actionsMaps == null)
        //            this.actionsMaps.AddRange(CommandParser.GetActionsMapsFromType(this.GetType(), this.OnlyMethodsWithAttribute, this.UsePrefixInAllMethods, this.PrefixMethods));
        //        return this.actionsMaps;
        //    }
        //}

        //public List<ArgumentMap> ArgumentsMaps
        //{
        //    get
        //    {
        //        if (this.argumentsMaps == null)
        //            this.argumentsMaps.AddRange(CommandParser.GetArgumentsMapsFromProperties(this.GetType(), this.OnlyPropertiesWithAttribute));
        //        return this.argumentsMaps;
        //    }
        //}

        //public IEnumerable<ActionMapped> Parse(IEnumerable<ArgumentRaw> argsRaw, bool enableMultiAction)
        //{
        //    return CommandParser.ParseActionMapped(argsRaw, enableMultiAction, this.ActionsMaps);
        //}

        //public IEnumerable<ArgumentMapped> Parse2(IEnumerable<ArgumentRaw> argsRaw, bool enablePositionalArgs)
        //{
        //    return CommandParser.ParseArgumentMapped(argsRaw, enablePositionalArgs, this.ArgumentsMaps);
        //}

    }
}
