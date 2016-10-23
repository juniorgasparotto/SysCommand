using System.Reflection;

namespace SysCommand.Evaluation
{
    public class MethodMainResult : IMemberResult
    {
        public string Name { get; private set; }
        public object Source { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public MethodMainResult(string name, object source, MethodInfo method)
        {
            this.Source = source;
            this.Name = name;
            this.MethodInfo = method;
        }

        public void Invoke()
        {
            this.Value = this.MethodInfo.Invoke(Source, null);
            this.IsInvoked = true;
        }
    }
}
