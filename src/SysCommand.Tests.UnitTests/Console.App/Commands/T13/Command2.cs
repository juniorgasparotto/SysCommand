using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;

namespace SysCommand.Tests.UnitTests.Commands.T13
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

        public string Delete(string value)
        {
            var cur = this.GetAction();
            return GetDebugName(this.CurrentActionMap(), cur) + "=" + value;
        }

        public string Delete(string value, string value2)
        {
            var cur = this.GetAction();
            return GetDebugName(this.CurrentActionMap(), cur) + "=" + value + "," + value2;
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
