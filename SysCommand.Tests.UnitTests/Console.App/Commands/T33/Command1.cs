using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T33
{
    public class Command1 : Command
    {
        [Argument(ShortName = 'c', LongName = "a1")]
        public string A1 { get; set; }

        [Action(IsDefault = true)]
        public string Method1(
            [Argument(ShortName = 'a', LongName = "p1")] int a,
            [Argument(ShortName = 'b', LongName = "p2")] int b
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
