using SysCommand.ConsoleApp;
using SysCommand.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class Command1 : SysCommand.ConsoleApp.Command
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
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Main(string[] args)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        [Action(IsDefault = true)]
        public string Default()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Delete()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        [Action(Name = "delete")]
        public string OtherName(int param = 0)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Save()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Save(int id)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        [Action(IsDefault=true)]
        public string Save(int id, string description = null, decimal? value = null)
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
