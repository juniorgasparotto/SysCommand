using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.UnitTests.Commands.T11
{
    public class Command2 : Command
    {
        public Command2()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main()");
        }

        public string Delete(int a)
        {
            var cur = this.CurrentMethodResult();
            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult()) + " Level" + cur.ActionParsed.Level;
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
