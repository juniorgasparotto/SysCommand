using SysCommand.Parsing;
using SysCommand.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand.Evaluation
{
    public class MethodResult : IMemberResult
    {
        public ActionParsed ActionParsed { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public string Name { get; private set; }
        public object Source { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public MethodResult(ActionParsed actionParsed)
        {
            this.ActionParsed = actionParsed;
            this.MethodInfo = this.ActionParsed.ActionMap.Method;
            this.Parameters = this.ActionParsed.Arguments.Where(f => f.IsMapped).ToDictionary(f => f.Name, f => f.Value);
            this.Source = this.ActionParsed.ActionMap.Source;
            this.Name = this.ActionParsed.Name;
        }

        public MethodResult(string name, string alias, object source, MethodInfo method, Dictionary<string, object> parameters)
        {
            this.MethodInfo = method;
            this.Parameters = parameters;
            this.Source = source;
            this.Name = name;
        }

        public void Invoke()
        {
            this.Value = this.MethodInfo.InvokeWithNamedParameters(Source, this.Parameters);
            this.IsInvoked = true;
        }
    }
}
