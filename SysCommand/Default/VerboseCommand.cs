using Fclp;
using System;
using System.Linq.Expressions;

namespace SysCommand
{
    //[Command(OrderExecution = -1)]
    public class VerboseCommand : Command<VerboseCommand.Arguments>
    {
        public VerboseCommand()
        {
            this.AllowSaveArgsInStorage = true;
            this.OrderExecution = -1;
        }

        public override void Execute()
        {
            var v = this.ArgsObject.Verbose.ToLower();
            ConsoleWriter.Verbose = 0;

            if (v.Contains("all"))
                ConsoleWriter.Verbose |= VerboseEnum.All;
            if (v.Contains("none"))
                ConsoleWriter.Verbose |= VerboseEnum.None;
            if (v.Contains("info"))
                ConsoleWriter.Verbose |= VerboseEnum.Info;
            if (v.Contains("success"))
                ConsoleWriter.Verbose |= VerboseEnum.Success;
            if (v.Contains("warning"))
                ConsoleWriter.Verbose |= VerboseEnum.Warning;
            if (v.Contains("critical"))
                ConsoleWriter.Verbose |= VerboseEnum.Critical;
            if (v.Contains("error"))
                ConsoleWriter.Verbose |= VerboseEnum.Error;
            if (v.Contains("question"))
                ConsoleWriter.Verbose |= VerboseEnum.Question;
            ConsoleWriter.Quiet = this.ArgsObject.Quiet;
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
