using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T12
{
    public class Command2 : Command
    {
        public Command2()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main()");
        }

        public string Delete(string value)
        {
            var cur = this.CurrentMethodResult();
            return GetDebugName(this.CurrentMethodMap(), cur) + "=" + value;
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
