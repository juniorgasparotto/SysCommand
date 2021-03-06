﻿using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.UnitTests.Common.Commands.T33
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
            return GetDebugName(this.GetActionMap(T<string, string, string, string>()), this.GetAction(T<string, string, string, string>()));
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
