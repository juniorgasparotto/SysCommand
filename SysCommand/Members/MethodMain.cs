using System.Reflection;

namespace SysCommand
{
    public class MethodMain : IMember
    {
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public object Source { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public MethodMain(string name, object source, MethodInfo method)
        {
            this.Source = source;
            this.Name = name;
            this.Alias = name;
            this.MethodInfo = method;
        }

        public void Invoke()
        {
            this.Value = this.MethodInfo.Invoke(Source, null);
            this.IsInvoked = true;
        }

    }
}
