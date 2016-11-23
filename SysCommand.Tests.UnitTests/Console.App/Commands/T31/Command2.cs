using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.UnitTests.Commands.T31
{
    public class Command2 : Command
    {
        public Command2()
        {
        }

        [Action(Help = "Loren ipsulum Loren ipsulum Loren ipsulum Loren")]
        public string Save(int value)
        {
            var cur = this.CurrentMethodResult();
            return GetDebugName(this.CurrentActionMap(), cur);
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
