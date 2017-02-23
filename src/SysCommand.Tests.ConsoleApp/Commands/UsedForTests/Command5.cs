//using System;
//using SysCommand.ConsoleApp;
//using SysCommand.Mapping;
//using SysCommand.Execution;

//namespace SysCommand.Tests.ConsoleApp.Commands
//{
//    public class Command5 : Command
//    {
//        public string Description { get; set; }

//        public Command5()
//        {
//            this.HelpText = "Command5: Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum Lorem ipsulum ";
//            this.EnablePositionalArgs = true;
//        }

//        public string Main()
//        {
//            return this.GetType().Name + ".Main";
//        }

//        public string Get(int description)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        public string Get(int id, int? description = null, int? other2 = null)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        public string Get(int id, string other = null, string other2 = null)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        private string GetDebugName(ActionMap map, ActionParsed parsed)
//        {
//            if (map != parsed.ActionMap)
//                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

//            var specification = Program.GetMethodSpecification(map);
//            return this.GetType().Name + "." + specification;
//        }
//    }
//}
