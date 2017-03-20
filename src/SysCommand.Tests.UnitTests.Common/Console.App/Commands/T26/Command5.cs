using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.UnitTests.Commands.T26
{
    public class Command5 : Command
    {
        public int PropertyValue { get; set; }

        public Command5()
        {
            this.EnablePositionalArgs = true;
        }

        [Action(IsDefault=true, EnablePositionalArgs=false)]
        public string Default(int value)
        {
            var cur = this.GetAction(T<int>());
            return GetDebugName(this.GetActionMap(T<int>()), cur);
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
