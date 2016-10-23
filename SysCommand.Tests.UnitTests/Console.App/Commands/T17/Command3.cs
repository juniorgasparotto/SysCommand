using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Evaluation;

namespace SysCommand.Tests.UnitTests.Commands.T17
{
    public class Command3 : Command
    {
        public decimal Price { get; set; }
        public void Main()
        {
            App.Console.Write("Price=" + Price);
        }

        public string Save(int? a = null)
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), cur);
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionMapped.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = App.MessageFormatter.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
