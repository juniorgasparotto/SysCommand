﻿using SysCommand.ConsoleApp;
using SysCommand.Execution;
using SysCommand.Mapping;
using System;
using System.Collections.Generic;

namespace SysCommand.Tests.UnitTests.Commands.T26
{
    public class Command8 : Command
    {
        public List<string> list { get; set; }

        public Command8()
        {
            this.EnablePositionalArgs = true;
        }

        [Action(IsDefault=true)]
        public string Default(List<int> lst, List<int> lst2)
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
