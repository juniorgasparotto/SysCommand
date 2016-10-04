namespace SysCommand.ConsoleApp
{
    //[Command(OrderExecution = -1)]
    public class VerboseCommand : CommandArguments<VerboseCommand.Arguments>
    {
        public VerboseCommand()
        {
            this.AllowSaveArgsInStorage = true;
            this.OrderExecution = -1;
        }

        public override void Execute()
        {
            var v = this.ArgsObject.Verbose.ToLower();
            App333.Current.Verbose = 0;
            App333.Current.Quiet = this.ArgsObject.Quiet;

            if (v.Contains("all"))
                App333.Current.Verbose |= VerboseEnum.All;
            if (v.Contains("none"))
                App333.Current.Verbose |= VerboseEnum.None;
            if (v.Contains("info"))
                App333.Current.Verbose |= VerboseEnum.Info;
            if (v.Contains("success"))
                App333.Current.Verbose |= VerboseEnum.Success;
            if (v.Contains("warning"))
                App333.Current.Verbose |= VerboseEnum.Warning;
            if (v.Contains("critical"))
                App333.Current.Verbose |= VerboseEnum.Critical;
            if (v.Contains("error"))
                App333.Current.Verbose |= VerboseEnum.Error;
            if (v.Contains("question"))
                App333.Current.Verbose |= VerboseEnum.Question;
        }

        #region Internal Parameters
        public class Arguments
        {
            [Argument(ShortName = 'v', LongName = "verbose", Help = "Specify the log level. The options are: all,info,success,warning,critical,error.", ShowHelpComplement = true, DefaultValue = "all")]
            public string Verbose { get; set; }

            [Argument(ShortName = 'q', LongName = "quiet", Help = "Display nothing in the output.", ShowHelpComplement = true)]
            public bool Quiet { get; set; }
        }
        #endregion
    }
}
