using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.UnitTests.Commands.T10
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
            return this.GetType().Name + string.Format(".Main() = Price={0}, Id={1}", Price, Id);
        }

        public string Save(int a, int b, int c)
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), cur) + " Level" + cur.ActionParsed.Level;
        }

        public string Save(int a, int b)
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), cur) + " Level" + cur.ActionParsed.Level;
        }

        public string Delete(int a)
        {
            var cur = this.CurrentMethodParse();
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse()) + " Level" + cur.ActionParsed.Level;
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionParsed.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = App.Descriptor.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
