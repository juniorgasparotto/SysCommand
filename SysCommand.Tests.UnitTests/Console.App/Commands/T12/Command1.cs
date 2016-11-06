using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.UnitTests.Commands.T12
{
    public class Command1 : Command
    {
        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main()");
        }

        public string Save()
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), cur);
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionParsed.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = CommandParserUtils.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
