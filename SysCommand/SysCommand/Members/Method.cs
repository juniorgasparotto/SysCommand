using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public class Method : IMember
    {
        public ActionMapped ActionMapped { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public string Name { get; private set; }
        public string Alias { get; private set; }
        public object Source { get; private set; }
        public object Value { get; private set; }
        public bool IsInvoked { get; private set; }

        //public int InvokePriority { get; private set; }

        public Method(ActionMapped actionMapped/*, int invokePriority*/)
        {
            this.ActionMapped = actionMapped;
            this.MethodInfo = this.ActionMapped.ActionMap.Method;
            this.Parameters = this.ActionMapped.ArgumentsMapped.Where(f => f.IsMapped).ToDictionary(f => f.Name, f => f.Value);
            this.Source = this.ActionMapped.ActionMap.Source;
            this.Name = this.ActionMapped.Name;
            this.Alias = this.ActionMapped.ActionMap.MapName;
            //this.InvokePriority = invokePriority;
        }

        public Method(string name, string alias, object source, MethodInfo method, Dictionary<string, object> parameters/*, int invokePriority*/)
        {
            this.MethodInfo = method;
            this.Parameters = parameters;
            this.Source = source;
            this.Name = name;
            this.Alias = alias;
            //this.InvokePriority = invokePriority;
        }

        public void Invoke()
        {
            this.Value = this.MethodInfo.InvokeWithNamedParameters(Source, this.Parameters);
            this.IsInvoked = true;
        }

    }
}
