using SysCommand.ConsoleApp;
using SysCommand.Execution;
using SysCommand.Mapping;
using System;

namespace SysCommand.Tests.UnitTests.Commands.T26
{
    public class Command4 : Command
    {
        public string PropertyValue { get; set; }

        public Command4()
        {
            this.EnablePositionalArgs = true;
        }

        [Action(IsDefault=true, EnablePositionalArgs=false)]
        public string Default(string value)
        {
            var cur = this.CurrentMethodResult();
            return GetDebugName(this.CurrentActionMap(), cur);
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
