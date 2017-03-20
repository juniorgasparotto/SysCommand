using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.UnitTests.Commands.T26
{
    public class Command1 : Command
    {
        public string Value { get; set; }

        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        [Action(IsDefault=true)]
        public string Default()
        {
            var cur = this.GetAction(T());
            return GetDebugName(this.GetActionMap(T()), cur);
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
