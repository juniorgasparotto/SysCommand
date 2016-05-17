using Fclp;
using System;

namespace SysCommand
{
    [CommandClassAttribute(OrderExecution = -1)]
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
        public class Arguments : IArguments
        {
            [CommandPropertyAttribute(ShortName='?', LongName="help", Help="Show help")]
            public bool Show { get; set; }

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
