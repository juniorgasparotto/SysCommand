using Fclp;
using System;
using System.Linq.Expressions;

namespace SysCommand
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
            App.Current.Verbose = 0;
            App.Current.Quiet = this.ArgsObject.Quiet;

            if (v.Contains("all"))
                App.Current.Verbose |= VerboseEnum.All;
            if (v.Contains("none"))
                App.Current.Verbose |= VerboseEnum.None;
            if (v.Contains("info"))
                App.Current.Verbose |= VerboseEnum.Info;
            if (v.Contains("success"))
                App.Current.Verbose |= VerboseEnum.Success;
            if (v.Contains("warning"))
                App.Current.Verbose |= VerboseEnum.Warning;
            if (v.Contains("critical"))
                App.Current.Verbose |= VerboseEnum.Critical;
            if (v.Contains("error"))
                App.Current.Verbose |= VerboseEnum.Error;
            if (v.Contains("question"))
                App.Current.Verbose |= VerboseEnum.Question;
        }

        #region Internal Parameters
        public class Arguments
        {
            [Argument(ShortName = 'v', LongName = "verbose", Help = "Specify the log level. The options are: all,info,success,warning,critical,error.", ShowDefaultValueInHelp = true, Default = "all")]
            public string Verbose { get; set; }

            [Argument(ShortName = 'q', LongName = "quiet", Help = "Display nothing in the output.", ShowDefaultValueInHelp = true)]
            public bool Quiet { get; set; }
        }
        #endregion
    }
}
