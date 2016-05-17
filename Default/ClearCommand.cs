using Fclp;
using System;
using System.Linq.Expressions;

namespace SysCommand
{
    [CommandClassAttribute(OrderExecution = -1)]
    public class ClearCommand : Command<ClearCommand.Arguments>
    {
        public override void Execute()
        {
            if (this.Args.Clear)
                Console.Clear();
        }

        #region Internal Parameters
        public class Arguments : IArguments
        {
            [CommandPropertyAttribute(LongName = "clear", Help = "Clear the prompt")]
            public bool Clear { get; set; }

            #region IArguments
            public string Command { get; set; }
            public string GetHelp(string propName)
            {
                return null;
            }
            #endregion
        }
        #endregion
    }
}
