using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.UnitTests.Commands.T05
{
    public class Command1 : Command
    {
        public Command1()
        {
        }

        public string Main()
        {
            return this.GetType().Name + ".Main";
        }

        public string Save(int a, int b, int c)
        {
            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
        }

        public string Save(int a, int b)
        {
            return GetDebugName(this.CurrentActionMap(), this.CurrentMethodResult());
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionParsed.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = CommandParserUtils.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
