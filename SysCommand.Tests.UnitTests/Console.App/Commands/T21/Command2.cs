﻿using SysCommand.ConsoleApp;
using SysCommand.Execution;
using SysCommand.Mapping;
using System;

namespace SysCommand.Tests.UnitTests.Commands.T21
{
    public class Command2 : Command
    {
        [Action(IsDefault = true)]
        public string Default()
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
