using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using System;

namespace SysCommand.Tests.UnitTests.Commands.T21
{
    public class Command3 : Command
    {
        [Action(IsDefault = true)]
        public string Main(string value = null, string value2 = null)
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
