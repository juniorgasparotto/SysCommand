using Fclp;
using System;

namespace SysCommand
{
    [Command(OrderExecution = -1)]
    public class HelperCommand : Command<HelperCommand.Arguments>
    {
        public override void Execute()
        {
            if (this.Args.Show)
                this.GetHelp();
        }

        public void GetHelp()
        {
            App.Current.ShowHelp();
        }
        
        #region Internal Parameters
        public class Arguments
        {
            [CommandPropertyAttribute(ShortName='?', LongName="help", Help="Show help")]
            public bool Show { get; set; }
        }
        #endregion        
    }
}
