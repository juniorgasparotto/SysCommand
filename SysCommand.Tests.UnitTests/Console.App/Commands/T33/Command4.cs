﻿using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.UnitTests.Commands.T33
{
    public class Command4 : Command
    {
        [Action(IsDefault = true)]
        public string Method4(
            [Argument(ShortName = 'c', LongName = "a1")] string c = "1",
            [Argument(ShortName = 'd', LongName = "a2")] string d = "2",
            [Argument(ShortName = 'a', LongName = "p1")] string a = "3",
            [Argument(ShortName = 'b', LongName = "p2")] string b = "4"
        )
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