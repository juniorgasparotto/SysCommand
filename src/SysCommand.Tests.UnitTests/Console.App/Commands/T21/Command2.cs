﻿using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System;

namespace SysCommand.Tests.UnitTests.Commands.T21
{
    public class Command2 : Command
    {
        [Action(IsDefault = true)]
        public string Default()
        {
            var cur = this.GetAction();
            return GetDebugName(this.CurrentActionMap(), cur);
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