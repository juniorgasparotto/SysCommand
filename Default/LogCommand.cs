using Fclp;
using System;
using System.Linq.Expressions;

namespace SysCommand
{
    [CommandClassAttribute(OrderExecution = -1)]
    public class LogCommand : Command<LogCommand.Arguments>
    {
        public override void Execute()
        {
            ConsoleWriter.Verbose = this.Args.Verbose;
            ConsoleWriter.Quiet = this.Args.Quiet;
        }

        #region Internal Parameters
        public class Arguments : IArguments
        {
            [CommandPropertyAttribute(ShortName = 'v', LongName = "verbose")]
            public string Verbose { get; set; }

            [CommandPropertyAttribute(ShortName = 'q', LongName = "quiet")]
            public bool Quiet { get; set; }

            #region IArguments
            public string Command { get; set; }
            public string GetHelp(string propName)
            {
                if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.Verbose).Name)
                    return string.Format("Specify the log level. The options are: none|success|info|critical|warning|error. Default is '{0}'", AppHelpers.GetDefaultValueForArgs<Arguments>(f => f.Verbose));
                else if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.Quiet).Name)
                    return string.Format("Display nothing in the output: Default is '{0}'", AppHelpers.GetDefaultValueForArgs<Arguments>(f => f.Quiet));
                return null;
            }
            #endregion
        }
        #endregion
    }
}
