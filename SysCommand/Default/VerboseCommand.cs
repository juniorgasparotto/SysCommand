using Fclp;
using System;
using System.Linq.Expressions;

namespace SysCommand
{
    [Command(OrderExecution = -1)]
    public class VerboseCommand : Command<VerboseCommand.Arguments>
    {
        public override void Execute()
        {
            ConsoleWriter.Verbose = this.Args.Verbose;
            ConsoleWriter.Quiet = this.Args.Quiet;
        }

        #region Internal Parameters
        public class Arguments : IHelp
        {
            [CommandPropertyAttribute(ShortName = 'v', LongName = "verbose", Default="all")]
            public string Verbose { get; set; }

            [CommandPropertyAttribute(ShortName = 'q', LongName = "quiet")]
            public bool Quiet { get; set; }

            #region IHelp
            public string GetHelp(string propName)
            {
                if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.Verbose).Name)
                    return string.Format("Specify the log level. The options are: none|success|info|critical|warning|error. Default is '{0}'", CommandStorage.GetValueForArgsType<Arguments>(f => f.Verbose));
                else if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.Quiet).Name)
                    return string.Format("Display nothing in the output: Default is '{0}'", CommandStorage.GetValueForArgsType<Arguments>(f => f.Quiet));
                return null;
            }
            #endregion
        }
        #endregion
    }
}
