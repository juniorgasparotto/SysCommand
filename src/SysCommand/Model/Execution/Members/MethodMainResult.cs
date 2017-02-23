using System.Reflection;

namespace SysCommand.Execution
{
    public class MethodMainResult : IMemberResult
    {
        public string Name { get; private set; }
        public object Target { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public MethodMainResult(string name, object target, MethodInfo method)
        {
            this.Target = target;
            this.Name = name;
            this.MethodInfo = method;
        }

        public void Invoke()
        {
            this.Value = this.MethodInfo.Invoke(Target, null);
            this.IsInvoked = true;
        }
    }
}
