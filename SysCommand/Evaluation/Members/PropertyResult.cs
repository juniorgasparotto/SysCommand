using SysCommand.Parsing;
using System.Reflection;

namespace SysCommand.Evaluation
{
    public class PropertyResult : IMemberResult
    {
        public ArgumentMapped ArgumentMapped { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public string Name { get; private set; }
        public object Source { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public PropertyResult(ArgumentMapped argumentMapped)
        {
            this.ArgumentMapped = argumentMapped;
            this.Name = this.ArgumentMapped.Name;

            if (argumentMapped.Map != null)
            { 
                this.PropertyInfo = (PropertyInfo)argumentMapped.Map.PropertyOrParameter;
                this.Value = argumentMapped.Value;
                this.Source = argumentMapped.Map.Source;
            }
        }

        public PropertyResult(string name, string alias, object source, PropertyInfo property, object value, int invokePriority)
        {
            this.PropertyInfo = property;
            this.Value = value;
            this.Source = source;
            this.Name = name;
        }

        public void Invoke()
        {
            this.PropertyInfo.SetValue(Source, this.Value);
            this.IsInvoked = true;
        }
    }
}
