using System.Collections.Generic;
using System;
using System.Reflection;

namespace SysCommand.Mapping
{
    /// <summary>
    /// Map of action.
    /// </summary>
    public class ActionMap
    {
        /// <summary>
        /// Represent the member name
        /// </summary>
        public string MapName { get; private set; }

        /// <summary>
        /// Action name with command prefix if exists
        /// </summary>
        public string ActionName { get; private set; }

        /// <summary>
        /// Action name without command prefix
        /// </summary>
        public string ActionNameRaw { get; private set; }

        /// <summary>
        /// Target instance (owner class)
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// MethodInfo instance
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Prefix if exists
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// Help text
        /// </summary>
        public string HelpText { get; private set; }

        /// <summary>
        /// Determines whether this action can have prefix of the command
        /// </summary>
        public bool UsePrefix { get; private set; }

        /// <summary>
        /// Type of method result, if exists
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// If true, this method may have its name omitted from the user input.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Enable positional inputs for your parameters.
        /// </summary>
        public bool EnablePositionalArgs { get; set; }

        /// <summary>
        /// All arguments of this action (all method parameter)
        /// </summary>
        public IEnumerable<ArgumentMap> ArgumentsMaps { get; private set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="target">Target instance (owner class)</param>
        /// <param name="method">MethodInfo instance</param>
        /// <param name="actionName">Action name with command prefix if exists</param>
        /// <param name="prefix">Prefix if exists</param>
        /// <param name="actionNameRaw">Action name without command prefix</param>
        /// <param name="usePrefix">Determines whether this action can have prefix of the command</param>
        /// <param name="helpText">Help text</param>
        /// <param name="isDefault">If true, this method may have its name omitted from the user input.</param>
        /// <param name="enablePositionalArgs">Enable positional inputs for your parameters.</param>
        /// <param name="argumentsMaps">All arguments of this action (all method parameter)</param>
        public ActionMap(object target, MethodInfo method, string actionName, string prefix, string actionNameRaw, bool usePrefix, string helpText, bool isDefault, bool enablePositionalArgs, IEnumerable<ArgumentMap> argumentsMaps)
        {
            this.Target = target;
            this.Method = method;
            this.MapName = method.Name;
            this.ActionName = actionName;
            this.ActionNameRaw = actionNameRaw;
            this.Prefix = prefix;
            this.HelpText = helpText;
            this.UsePrefix = usePrefix;
            this.ReturnType = method.ReturnType;
            this.ArgumentsMaps = new List<ArgumentMap>(argumentsMaps);
            this.IsDefault = isDefault;
            this.EnablePositionalArgs = enablePositionalArgs;
        }

        public override string ToString()
        {
            return "[" + this.MapName + ", " + this.Target + "]";
        }
    }
}
