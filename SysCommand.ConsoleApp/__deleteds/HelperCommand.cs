//namespace SysCommand.ConsoleApp
//{
//    //[Command(OrderExecution = -1)]
//    public class HelperCommand : CommandArguments<HelperCommand.Arguments>
//    {
//        public HelperCommand()
//        {
//            this.AllowSaveArgsInStorage = true;
//            this.OrderExecution = -1;
//        }

//        public override void Execute()
//        {
//            if (this.ArgsObject.Show)
//            {
//                App333.Current.ShowHelp();
//                App333.Current.ShowHelp2();
//                App333.Current.StopPropagation();
//            }
//        }
        
//        #region Internal Parameters
//        public class Arguments
//        {
//            [Argument(ShortName = '?', LongName = "help", Help = "Show help")]
//            public bool Show { get; set; }
//        }
//        #endregion        
//    }
//}
