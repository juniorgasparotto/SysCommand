using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class Command3 : SysCommand.ConsoleApp.Command
    {
        public bool ShowTime { get; set; }

        public Command3()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + ".Main";
        }

        public string Main(string value)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        [Action(IsDefault = true)]
        public string Default(string value)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Save()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Delete(string title = null, string description = null, DateTime? date = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        private string GetDebugName(ActionMap map, Method result)
        {
            if (map != result.ActionMapped.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = DefaultEventListener.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
