using SysCommand.ConsoleApp;
using SysCommand.Execution;
using SysCommand.Mapping;
using System;
using System.Collections.Generic;
namespace SysCommand.Tests.UnitTests.Commands.T35
{
    public class Command2 : Command
    {
        [Action(IsDefault = true)]
        public string Method1(
            [Argument(ShortName = 'a', LongName = "a1")] string a, string b, string c, string d
        )
        {
            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
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
