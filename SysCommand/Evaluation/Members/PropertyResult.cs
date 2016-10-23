using SysCommand.Parsing;
using System.Reflection;

namespace SysCommand.Evaluation
{
    public class PropertyResult : IMemberResult
    {
        public ArgumentParsed ArgumentParsed { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public string Name { get; private set; }
        public object Source { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public PropertyResult(ArgumentParsed argumentParsed)
        {
            this.ArgumentParsed = argumentParsed;
            this.Name = this.ArgumentParsed.Name;

            if (argumentParsed.Map != null)
            { 
                this.PropertyInfo = (PropertyInfo)argumentParsed.Map.PropertyOrParameter;
                this.Value = argumentParsed.Value;
                this.Source = argumentParsed.Map.Source;
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
