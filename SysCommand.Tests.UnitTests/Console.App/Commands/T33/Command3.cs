using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Execution;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T33
{
    public class Command3 : Command
    {
        [Action(IsDefault = true)]
        public string Method3(
            [Argument(ShortName = 'a', LongName = "p1")] int a,
            [Argument(ShortName = 'b', LongName = "p2")] int b = 2
        )
        {
            return GetDebugName(this.CurrentActionMap(), this.GetAction());
        }

        private string GetDebugName(ActionMap map, ActionParsed parsed)
        {
            if (map != parsed.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = CommandParserUtils.GetMethodSpecification(parsed);
            return this.GetType().Name + "." + specification;
        }
    }
}
