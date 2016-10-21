using SysCommand.ConsoleApp;
using SysCommand.Parser;
using System;

namespace SysCommand.Tests.UnitTests.Commands.T21
{
    public class Command1 : Command
    {
        [Action(IsDefault=true)]
        public string Default()
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), cur);
        }

        private string GetDebugName(ActionMap map, Method result)
        {
            if (map != result.ActionMapped.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = App.MessageFormatter.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
