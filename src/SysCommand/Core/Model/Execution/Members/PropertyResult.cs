using SysCommand.Parsing;
using System.Reflection;

namespace SysCommand.Execution
{
    /// <summary>
    /// Result of properties that represent arguments.
    /// </summary>
    public class PropertyResult : IMemberResult
    {
        /// <summary>
        /// ArgumentParsed of this method
        /// </summary>
        public ArgumentParsed ArgumentParsed { get; private set; }

        /// <summary>
        /// PropertyInfo for this result
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

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
        /// Invoke member
        /// </summary>
        public bool IsInvoked { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="argumentParsed">ArgumentParsed of this method</param>
        public PropertyResult(ArgumentParsed argumentParsed)
        {
            this.ArgumentParsed = argumentParsed;
            this.Name = this.ArgumentParsed.Name;

            if (argumentParsed.Map != null)
            { 
                this.PropertyInfo = (PropertyInfo)argumentParsed.Map.TargetMember;
                this.Value = argumentParsed.Value;
                this.Target = argumentParsed.Map.Target;
            }
        }

        /// <summary>
        /// Invoke member
        /// </summary>
        public void Invoke()
        {
            this.PropertyInfo.SetValue(Target, this.Value);
            this.IsInvoked = true;
        }
    }
}
