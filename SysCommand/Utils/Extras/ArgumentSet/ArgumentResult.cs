using SysCommand.Evaluation;
using SysCommand.Parsing;
using System.Reflection;

namespace SysCommand.Utils.Extras
{
    public class ArgumentResult : IMemberResult
    {
        public ArgumentParsed ArgumentParsed { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public string Name { get; private set; }
        public object Target { get; private set; }
        public object Value { get; set; }
        public bool IsInvoked { get; set; }

        public ArgumentResult(ArgumentParsed argumentParsed)
        {
            this.ArgumentParsed = argumentParsed;
            this.Name = this.ArgumentParsed.Name;

            if (argumentParsed.Map != null)
            { 
                this.MethodInfo = (MethodInfo)argumentParsed.Map.TargetMember;
                this.Value = argumentParsed.Value;
                this.Target = argumentParsed.Map.Target;
            }
        }

        public void Invoke()
        {
            this.MethodInfo.Invoke(Target, new[] { this.Value });
            this.IsInvoked = true;
        }
    }
}
