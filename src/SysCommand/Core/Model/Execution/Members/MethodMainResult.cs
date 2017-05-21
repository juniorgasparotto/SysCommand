using System.Reflection;

namespace SysCommand.Execution
{
    /// <summary>
    /// Result for methods with the signature "Main()"
    /// </summary>
    public class MethodMainResult : IMemberResult
    {
        /// <summary>
        /// Member name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Member Target  (owner class)
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// MethodInfo for this result
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

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
        /// <param name="name">Member name</param>
        /// <param name="target">Member Target  (owner class)</param>
        /// <param name="method">MethodInfo for this result</param>
        public MethodMainResult(string name, object target, MethodInfo method)
        {
            this.Target = target;
            this.Name = name;
            this.MethodInfo = method;
        }

        /// <summary>
        /// Invoke member
        /// </summary>
        public void Invoke()
        {
            this.Value = this.MethodInfo.Invoke(Target, null);
            this.IsInvoked = true;
        }
    }
}
