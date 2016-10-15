using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands
{
    public class Command7 : Command
    {
        public string Description { get; set; }

        public Command7()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main(int a, int b, int c)
        {
            return this.GetType().Name + ".Main";
        }

        public string Save(int a, int b, int c)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodResult());
        }

        public string Save(int a, int b)
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
