﻿using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Execution;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T11
{
    public class Command3 : Command
    {
        public decimal Price { get; set; }
        public int Id { get; set; }

        public Command3()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main() = Price={0}, Id={1}", Price, Id);
        }

        public string Save(int a, int b)
        {
            var cur = this.GetAction();
            return GetDebugName(this.CurrentActionMap(), cur) + " Level" + cur.Level;
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