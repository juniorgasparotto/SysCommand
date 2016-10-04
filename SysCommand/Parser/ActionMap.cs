using System.Collections.Generic;
using System;
using System.Reflection;

namespace SysCommand.Parser
{
    public class ActionMap
    {
        public string MapName { get; private set; }
        public string ActionName { get; private set; }
        public string ActionNameRaw { get; private set; }
        public object Source { get; private set; }
        public MethodInfo Method { get; private set; }
        public string Prefix { get; private set; }
        public bool UsePrefix { get; private set; }
        public Type ReturnType { get; private set; }
        public bool IsDefault { get; set; }
        public bool EnablePositionalArgs { get; set; }
        public IEnumerable<ArgumentMap> ArgumentsMaps { get; private set; }

        public ActionMap(object source, MethodInfo method, string actionName, string prefix, string actionNameRaw, bool usePrefix, bool isDefault, bool enablePositionalArgs, IEnumerable<ArgumentMap> argumentsMaps)
        {
            this.Source = source;
            this.Method = method;
            this.MapName = method.Name;
            this.ActionName = actionName;
            this.ActionNameRaw = actionNameRaw;
            this.Prefix = prefix;
            this.UsePrefix = usePrefix;
            this.ReturnType = method.ReturnType;
            this.ArgumentsMaps = new List<ArgumentMap>(argumentsMaps);
            this.IsDefault = isDefault;
            this.EnablePositionalArgs = enablePositionalArgs;
        }

        public override string ToString()
        {
            return "[" + this.MapName + ", " + this.Source + "]";
        }
    }
}
