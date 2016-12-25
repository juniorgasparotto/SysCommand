//using SysCommand.ConsoleApp;
//using SysCommand.Execution;
//using SysCommand.Mapping;
//using System;

//namespace SysCommand.Tests.ConsoleApp.Commands
//{
//    public class Command1 : Command
//    {
//        [Argument(Help = "Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum ")]
//        public int Id { get; set; }

//        public Command1()
//        {
//            this.EnablePositionalArgs = true;
//        }

//        public string Main()
//        {
//            return this.GetType().Name + ".Main";
//        }

//        public string Main(int id, string p2)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        public string Main(string[] args)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        [Action(IsDefault = true, Help = "Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum")]
//        public string Default()
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        [Action(Help = "Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum")]
//        public string Delete()
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        [Action(Name = "delete", Help = "Lorem ipsulum Lorem ipsulum Lorem")]
//        public string OtherName(int param = 0)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        [Action(Name = "delete", Help = "Lorem ipsulum Lorem ipsulum Lorem")]
//        public string Save()
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        [Action(Name = "delete", Help = "Lorem ipsulum Lorem ipsulum Lorem")]
//        public string Save(int id)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        [Action(IsDefault=true, Help = "Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum ")]
//        public string Save(
//            [Argument(LongName = "Identification", Help = "Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum ")]
//            int id,
//            [Argument(Help = "Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum ")]
//            string description = null,
//            [Argument(Help = "Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum ")]
//            decimal? value = null)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        private string GetDebugName(ActionMap map, MethodResult result)
//        {
//            if (map != result.ActionParsed.ActionMap)
//                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

//            var specification = Program.GetMethodSpecification(map);
//            return this.GetType().Name + "." + specification;
//        }
//    }
//}
