using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.UnitTests.Commands.T26
{
    public class Command2 : Command
    {
        public string Value { get; set; }

        public Command2()
        {
            this.EnablePositionalArgs = true;
        }

        [Action(IsDefault=true, EnablePositionalArgs=false)]
        public string Default(string value)
        {
            var cur = this.GetAction(T<string>());
            return GetDebugName(this.GetActionMap(T<string>()), cur);
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
