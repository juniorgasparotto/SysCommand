﻿using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T17
{
    public class Command2 : Command
    {
        public string Save(int? a = null)
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