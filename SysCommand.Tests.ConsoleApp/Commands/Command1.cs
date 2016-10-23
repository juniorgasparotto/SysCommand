using SysCommand.ConsoleApp;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class Command1 : Command
    {
        public int Id { get; set; }

        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + ".Main";
        }

        public string Main(int id, string p2)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Main(string[] args)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        [Action(IsDefault = true)]
        public string Default()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Delete()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        [Action(Name = "delete")]
        public string OtherName(int param = 0)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Save()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Save(int id)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        [Action(IsDefault=true)]
        public string Save(int id, string description = null, decimal? value = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionParsed.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = App.MessageFormatter.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
