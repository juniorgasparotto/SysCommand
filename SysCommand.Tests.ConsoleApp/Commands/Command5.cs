using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Evaluation;

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
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Get(int id, int? description = null, int? other2 = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Get(int id, string other = null, string other2 = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionMapped.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = App.MessageFormatter.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
