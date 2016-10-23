﻿using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parser;

namespace SysCommand.Tests.UnitTests.Commands.T14
{
    public class Command2 : Command
    {
        public decimal Price { get; set; }
        public int Id { get; set; }

        public Command2()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main()");
        }

        public string Delete(int value)
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), cur) + "=" + value;
        }

        public string Delete(int value, int value2)
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), cur) + "=" + value + "," + value2;
        }

        private string GetDebugName(ActionMap map, Method result)
        {
            if (map != result.ActionMapped.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = App.MessageFormatter.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}