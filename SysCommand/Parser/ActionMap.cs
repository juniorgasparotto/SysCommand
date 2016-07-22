using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace SysCommand
{
    public class ActionMap
    {
        public string MapName { get; private set; }
        public string ActionName { get; private set; }
        public string ActionNameRaw { get; private set; }
        public MethodInfo Method { get; private set; }
        public string Prefix { get; private set; }
        public bool UsePrefix { get; private set; }
        public Type ReturnType { get; private set; }
        public Type ParentClassType { get; private set; }
        public bool IsDefault { get; set; }
        public bool EnablePositionalArgs { get; set; }
        public IEnumerable<ArgumentMap> ArgumentsMaps { get; private set; }

        public ActionMap(MethodInfo method, string actionName, string prefix, string actionNameRaw, bool usePrefix, Type parentClassType, bool isDefault, bool enablePositionalArgs, IEnumerable<ArgumentMap> argumentsMaps)
        {
            this.MapName = method.Name;
            this.ActionName = actionName;
            this.ActionNameRaw = actionNameRaw;
            this.Method = method;
            this.Prefix = prefix;
            this.UsePrefix = usePrefix;
            this.ReturnType = method.ReturnType;
            this.ParentClassType = parentClassType;
            this.ArgumentsMaps = new List<ArgumentMap>(argumentsMaps);
            this.IsDefault = isDefault;
            this.EnablePositionalArgs = enablePositionalArgs;
        }

        public override string ToString()
        {
            return "[" + this.MapName + ", " + this.ParentClassType + "]";
        }
    }
}
