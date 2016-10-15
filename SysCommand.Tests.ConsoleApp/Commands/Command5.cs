using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class Command5 : Command
    {
        public string Description { get; set; }

        public Command5()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + ".Main";
        }

        public string Get(int description)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Get(int id, int? description = null, int? other2 = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Get(int id, string other = null, string other2 = null)
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
