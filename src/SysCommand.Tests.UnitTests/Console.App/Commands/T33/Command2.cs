using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;

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
