using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.UnitTests.Commands.T33
{
    public class Command2 : Command
    {
        [Argument(ShortName = 'd', LongName = "description")]
        public string D { get; set; }

        [Action(IsDefault = true)]
        public string Method2(
            [Argument(ShortName = 'a', LongName = "p1")] string a,
            [Argument(ShortName = 'b', LongName = "p2")] string b
        )
        {
            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionParsed.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = CommandParserUtils.GetMethodSpecification(result);
            return this.GetType().Name + "." + specification;
        }
    }
}
