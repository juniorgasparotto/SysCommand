//using System;
//using SysCommand.ConsoleApp;
//using SysCommand.Mapping;
//using SysCommand.Execution;

//namespace SysCommand.Tests.ConsoleApp.Commands
//{
//    public class Command3 : Command
//    {
//        public bool ShowTime { get; set; }

//        public Command3()
//        {
//            this.EnablePositionalArgs = true;
//        }

//        public string Main()
//        {
//            return this.GetType().Name + ".Main";
//        }

//        public string Main(string value)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        [Action(IsDefault = true)]
//        public string Default(string value)
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        public string Save()
//        {
//            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
//        }

//        public string Delete(string title = null, string description = null, DateTime? date = null)
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
