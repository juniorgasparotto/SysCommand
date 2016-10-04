using Fclp;
using System;
using System.Linq.Expressions;

namespace SysCommand
{
    //[Command(OrderExecution = -1, OnlyInDebug = true)]
    public class ClearCommand : CommandArguments<ClearCommand.Arguments>
    {
        public ClearCommand()
        {
            this.OrderExecution = -1;
            this.OnlyInDebug = true;
        }

        public override void Execute()
        {
            if (this.ArgsObject.Clear)
            {
                Console.Clear();
                App.Current.StopPropagation();
            }
        }

        #region Internal Parameters
        public class Arguments
        {
            [Argument(LongName = "clear", Help = "Clear the prompt")]
            public bool Clear { get; set; }
        }
        #endregion
    }
}
