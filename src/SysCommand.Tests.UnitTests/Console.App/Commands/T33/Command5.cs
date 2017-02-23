using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T33
{
    public class Command5 : Command
    {
        [Action(IsDefault = true)]
        public string Method5(
            [Argument(ShortName = 'a', LongName = "p1")] string a,
            [Argument(ShortName = 'c', LongName = "p2")] string c,
            [Argument(ShortName = 'e', LongName = "p3")] string e = "1"
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
