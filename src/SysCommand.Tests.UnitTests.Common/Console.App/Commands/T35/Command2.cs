using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.UnitTests.Common.Commands.T35
{
    public class Command2 : Command
    {
        [Action(IsDefault = true)]
        public string Method1(
            [Argument(ShortName = 'a', LongName = "a1")] string a, string b, string c, string d
        )
        {
            return GetDebugName(this.GetActionMap(T<string, string, string, string>()), this.GetAction(T<string, string, string, string>()));
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
