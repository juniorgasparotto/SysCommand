using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Parsing;
using static SysCommand.Helpers.ReflectionHelper;

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
            var cur = this.GetAction(T<int?>());
            return GetDebugName(this.GetActionMap(T<int?>()), cur);
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
