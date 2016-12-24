using SysCommand.ConsoleApp;
using SysCommand.Execution;
using SysCommand.Mapping;
using System;

namespace SysCommand.Tests.UnitTests.Commands.T26
{
    public class Command3 : Command
    {
        public string Value { get; set; }

        public Command3()
        {
            this.EnablePositionalArgs = true;
        }

        [Action(IsDefault=true)]
        public string Default(string value = null)
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
