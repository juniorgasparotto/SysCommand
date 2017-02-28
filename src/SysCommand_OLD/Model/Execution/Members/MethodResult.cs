using SysCommand.Parsing;
using SysCommand.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand.Execution
{
    public class MethodResult : IMemberResult
    {
        public ActionParsed ActionParsed { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public string Name { get; private set; }
        public object Target { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public MethodResult(ActionParsed actionParsed)
        {
            this.ActionParsed = actionParsed;
            this.MethodInfo = this.ActionParsed.ActionMap.Method;
            this.Parameters = this.ActionParsed.Arguments.Where(f => f.IsMapped).ToDictionary(f => f.Name, f => f.Value);
            this.Target = this.ActionParsed.ActionMap.Target;
            this.Name = this.ActionParsed.Name;
        }

        public MethodResult(string name, string alias, object target, MethodInfo method, Dictionary<string, object> parameters)
        {
            this.MethodInfo = method;
            this.Parameters = parameters;
            this.Target = target;
            this.Name = name;
        }

        public void Invoke()
        {
            this.Value = this.MethodInfo.InvokeWithNamedParameters(Target, this.Parameters);
            this.IsInvoked = true;
        }
    }
}
