using SysCommand.Parsing;
using System.Reflection;

namespace SysCommand.Evaluation
{
    public class PropertyResult : IMemberResult
    {
        public ArgumentParsed ArgumentParsed { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public string Name { get; private set; }
        public object Target { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

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

        public PropertyResult(string name, string alias, object target, PropertyInfo property, object value, int invokePriority)
        {
            this.PropertyInfo = property;
            this.Value = value;
            this.Target = target;
            this.Name = name;
        }

        public void Invoke()
        {
            this.PropertyInfo.SetValue(Target, this.Value);
            this.IsInvoked = true;
        }
    }
}
