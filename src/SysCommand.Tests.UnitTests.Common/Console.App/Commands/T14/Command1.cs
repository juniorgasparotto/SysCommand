using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.UnitTests.Common.Commands.T14
{
    public class Command1 : Command
    {
        public decimal Price { get; set; }
        public int Id { get; set; }

        public Command1()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + string.Format(".Main()");
        }

        public string Save(int? a = null)
        {
            var cur = this.GetAction(T<int?>());
            return GetDebugName(this.GetActionMap(T<int?>()), cur);
        }

        public string Delete(int value)
        {
            var cur = this.GetAction(T<int>());
            return GetDebugName(this.GetActionMap(T<int>()), cur) + "=" + value;
        }

        public string Delete(int value, int value2)
        {
            var cur = this.GetAction(T<int, int>());
            return GetDebugName(this.GetActionMap(T<int, int>()), cur) + "=" + value + "," + value2;
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
