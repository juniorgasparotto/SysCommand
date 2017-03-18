using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System;
using System.Collections.Generic;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.UnitTests.Commands.T35
{
    public class Command1 : Command
    {
        [Action(IsDefault = true)]
        public string Method1(
            [Argument(ShortName = 'a', LongName = "p1")] List<string> a
        )
        {
            return GetDebugName(this.GetActionMap(T<List<string>>()), this.GetAction(T<List<string>>()));
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
