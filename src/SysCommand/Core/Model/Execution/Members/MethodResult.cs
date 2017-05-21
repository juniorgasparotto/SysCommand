using SysCommand.Parsing;
using SysCommand.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand.Execution
{
    /// <summary>
    /// Result of methods that represent actions.
    /// </summary>
    public class MethodResult : IMemberResult
    {
        /// <summary>
        /// ActionParsed of this method
        /// </summary>
        public ActionParsed ActionParsed { get; private set; }

        /// <summary>
        /// MethodInfo for this result
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// All parameters of this method
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Member value
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Member Target  (owner class)
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// Member value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Checks if member was invoked
        /// </summary>
        public bool IsInvoked { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="actionParsed">ActionParsed of this method</param>
        public MethodResult(ActionParsed actionParsed)
        {
            this.ActionParsed = actionParsed;
            this.MethodInfo = this.ActionParsed.ActionMap.Method;
            this.Parameters = this.ActionParsed.Arguments.Where(f => f.IsMapped).ToDictionary(f => f.Name, f => f.Value);
            this.Target = this.ActionParsed.ActionMap.Target;
            this.Name = this.ActionParsed.Name;
        }

        /// <summary>
        /// Invoke member
        /// </summary>
        public void Invoke()
        {
            this.Value = this.MethodInfo.InvokeWithNamedParameters(Target, this.Parameters);
            this.IsInvoked = true;
        }
    }
}
