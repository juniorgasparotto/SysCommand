using Fclp;
using System;

namespace SysCommand
{
    [Command(OrderExecution = -1)]
    public class HelperCommand : Command<HelperCommand.Arguments>
    {
        public HelperCommand()
        {
            this.AllowSaveArgsInStorage = true;
        }

        public override void Execute()
        {
            if (this.Args.Show)
            {
                App.Current.ShowHelp();
                App.Current.StopPropagation();
            }
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
